using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EGameState
{
    Loading,
    MainMenu,
    OnlineMenu,
    Lobby,
    InGame_Offline,
    InGame_Online,
    Results
}


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    [SerializeField] private SceneTransitionHandler transitionHandler;

    public static GameManager Instance => _instance;

    public Persistance_Data PersistanceData { get; private set; } = new();
    public SceneTransitionHandler TransitionHandler => transitionHandler;

    public EGameState CurrentGameState { get; private set; }
    
    public Action<EGameState, EGameState> OnGameStateChange;

    private string _sceneToUnloadAfterLoad;

    private const string SCENE_MAIN = "0_MainScene";
    private const string SCENE_MENU = "1_MenuScene";
    private const string SCENE_GAME = "2_GameScene";
    private const string SCENE_ONLINE_MENU = "3_OnlineMainMenu";
    private const string SCENE_LOBBY_MENU = "4_LobbyMenu";
    private const string SCENE_ONLINE_GAME = "10_OnlineGameScene";

    private EGameState _pendingNetworkState;

    private bool _returningToMenu = false;
    

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("There are other instance of GameManager");
            Destroy(gameObject);

        }
    }

    private async void Start()
    {
        try
        {
            await GameServiceLocator.GetService<RelayManager>().InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameManager] Error inicializando Relay: {e.Message}");
        }

        GameServiceLocator.GetService<ConnectionCallBackManager>().Subscribe();

        var saveSystem = GameServiceLocator.GetService<ISaveLoadSystem>();
        if (saveSystem.TryLoad(out GameMain_Data data))
            PersistanceData.Initialize(data);
        else
            PersistanceData.CreateNewGameData();

        RequestMainMenuScene();
    }

    public void ChangeGameState(EGameState newState)
    {
        EGameState oldState = CurrentGameState;
        CurrentGameState = newState;
        OnGameStateChange?.Invoke(oldState, newState);
    }


    public void RequestMainMenuScene()
    {
        transitionHandler.TransitionTo(SCENE_MENU, new string[] { SCENE_MAIN }, EGameState.MainMenu);
    }

    public void RequestGameScene()
    {
        transitionHandler.TransitionTo(SCENE_GAME, new string[] { SCENE_MAIN }, EGameState.InGame_Offline);
    }

    public void RequestOnlineMenuScene()
    {
        transitionHandler.TransitionTo(SCENE_ONLINE_MENU, new string[] { SCENE_MAIN }, EGameState.OnlineMenu);
    }

    public void ReturnToOnlineMenuAfterSession()
    {
        if (_returningToMenu) return;
        _returningToMenu = true;
        StartCoroutine(ReturnToOnlineMenuCoroutine());
    }

    private IEnumerator ReturnToOnlineMenuCoroutine()
    {
        // Esperar hasta que NetworkManager confirme shutdown completo
        yield return new WaitUntil(() => 
            !NetworkManager.Singleton.IsListening && 
            !NetworkManager.Singleton.ShutdownInProgress);

        var unloadOps = new List<AsyncOperation>();
        AsyncOperation lobbyUnload = UnloadSceneIfLoaded(SCENE_LOBBY_MENU);
        AsyncOperation gameUnload  = UnloadSceneIfLoaded(SCENE_ONLINE_GAME);
        if (lobbyUnload != null) unloadOps.Add(lobbyUnload);
        if (gameUnload  != null) unloadOps.Add(gameUnload);

        foreach (var op in unloadOps)
            yield return op;

        _returningToMenu = false;
        RequestOnlineMenuScene();
    }

    private AsyncOperation UnloadSceneIfLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.IsValid() && scene.isLoaded)
            return SceneManager.UnloadSceneAsync(scene);
        return null;
    }

    public void RequestOnlineLobbyScene()
    {
        GameServiceLocator.GetService<LoadScreen_System>().ToggleLoaderScreen(true);
        ChangeGameState(EGameState.Loading);
        _pendingNetworkState = EGameState.Lobby;
        _sceneToUnloadAfterLoad = SCENE_ONLINE_MENU;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnNetworkSceneLoaded;
        NetworkManager.Singleton.SceneManager.LoadScene(SCENE_LOBBY_MENU, LoadSceneMode.Additive);
    }

    public void RequestOnlineGameScene() 
    {
        GameServiceLocator.GetService<LoadScreen_System>().ToggleLoaderScreen(true);
        ChangeGameState(EGameState.Loading);
        _pendingNetworkState    = EGameState.InGame_Online; // ← corregido
        _sceneToUnloadAfterLoad = SCENE_LOBBY_MENU;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnNetworkSceneLoaded;
        NetworkManager.Singleton.SceneManager.LoadScene(SCENE_ONLINE_GAME, LoadSceneMode.Additive); // ← corregido
    }

    public void RequestResultsScene() 
    {
        transitionHandler.TransitionTo(SCENE_MENU, new[] { SCENE_MAIN }, EGameState.Results);
    }



    private void OnNetworkSceneLoaded(
    string sceneName,
    LoadSceneMode mode,
    List<ulong> clientsCompleted,
    List<ulong> clientsTimedOut)
    {
        
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnNetworkSceneLoaded;

        if (clientsTimedOut.Count > 0)
            Debug.LogWarning($"[GameManager] {clientsTimedOut.Count} cliente(s) tardaron demasiado cargando {sceneName}");

        if (!string.IsNullOrEmpty(_sceneToUnloadAfterLoad))
        {
            Scene toUnload = SceneManager.GetSceneByName(_sceneToUnloadAfterLoad);
            if (toUnload.IsValid() && toUnload.isLoaded)
            {
                // Si la cargó Netcode → la descarga Netcode
                // Si la cargó SceneTransitionHandler → la descarga Unity directamente
                if (_sceneToUnloadAfterLoad == SCENE_ONLINE_MENU)
                    SceneManager.UnloadSceneAsync(toUnload); // carga local
                else
                    NetworkManager.Singleton.SceneManager.UnloadScene(toUnload); // carga de red
            }
            _sceneToUnloadAfterLoad = null;
        }

        Scene loaded = SceneManager.GetSceneByName(sceneName);
        if (loaded.IsValid() && loaded.isLoaded)
            SceneManager.SetActiveScene(loaded);

        ChangeGameState(_pendingNetworkState);
        GameServiceLocator.GetService<LoadScreen_System>().ToggleLoaderScreen(false);
    }



    
}
