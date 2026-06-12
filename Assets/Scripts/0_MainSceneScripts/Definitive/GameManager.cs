using System;
using UnityEditorInternal;
using UnityEngine;

public enum EGameState
{
    Loading = 0,
    OnMenu = 1,
    OnGame = 2
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

    private const string SCENE_MAIN = "0_MainScene";
    private const string SCENE_MENU = "1_MenuScene";
    private const string SCENE_GAME = "2_GameScene";
    private const string SCENE_ONLINE_MENU = "3_OnlineMainMenu";
    

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogError("There are other instance of GameManager");
            Destroy(gameObject);

        }
    }

    private void Start()
    {
        var saveSystem = GameServiceLocator.GetService<ISaveLoadSystem>();

        if (saveSystem.TryLoad(out GameMain_Data data))
            PersistanceData.Initialize(data);
        else
            PersistanceData.CreateNewGameData();

        RequestMainMenuScene    ();
    }

    public void ChangeGameState(EGameState newState)
    {
        EGameState oldState = CurrentGameState;
        CurrentGameState = newState;
        OnGameStateChange?.Invoke(oldState, newState);
    }

    public void RequestMainMenuScene()
    {
        transitionHandler.TransitionTo(SCENE_MENU, new string[] { SCENE_MAIN }, EGameState.OnMenu);
    }

    public void RequestGameScene()
    {
        transitionHandler.TransitionTo(SCENE_GAME, new string[] { SCENE_MAIN }, EGameState.OnGame);
    }

    public void RequestOnlineMenuScene()
    {
        transitionHandler.TransitionTo(SCENE_ONLINE_MENU, new string[] { SCENE_MAIN }, EGameState.OnMenu);
    }

    
}
