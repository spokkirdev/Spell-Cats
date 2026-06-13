using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class ConnectionCallBackManager : MonoBehaviour
{


    private void Awake()
    {
        GameServiceLocator.RegisterService(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<ConnectionCallBackManager>(); // falta esto
        Unsubscribe();
    }
    public void Subscribe()
    {
        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientStarted += MethodOnClientStarted;
            NetworkManager.Singleton.OnClientStopped += MethodOnClientStopped;
            NetworkManager.Singleton.OnClientConnectedCallback += MethodOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += MethodOnClientDisconnect;
        }
    }

    public void Unsubscribe()
    {
        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientStarted -= MethodOnClientStarted;
            NetworkManager.Singleton.OnClientStopped -= MethodOnClientStopped;
            NetworkManager.Singleton.OnClientConnectedCallback -= MethodOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= MethodOnClientDisconnect;
        }
    }

    private void MethodOnClientStarted()
    {
        Debug.Log("Client Started");
    }

    private void MethodOnClientStopped(bool isHost)
    {
        Debug.Log("Client Stopped, isHost: " + isHost);
        GameManager.Instance.ReturnToOnlineMenuAfterSession();
    }
    

    private void MethodOnClientConnected(ulong playerId)
    {
        Debug.Log($"[MethodOnClientConnected] playerId: {playerId}, localId: {NetworkManager.Singleton.LocalClientId}, isHost: {NetworkManager.Singleton.IsHost}");

        if (NetworkManager.Singleton.IsHost)
        {
            if (NetworkManager.Singleton.LocalClientId != playerId) return;
            StartCoroutine(LoadLobbyNextFrame());
        }
        else
        {
            // La escena ya está cargada en este punto
            // Solo necesitamos descargar el OnlineMenu
            StartCoroutine(ClientCleanupAfterJoin());
        }
    }

    private IEnumerator ClientCleanupAfterJoin()
    {
        yield return new WaitUntil(() => 
        NetworkManager.Singleton.IsConnectedClient &&
        !NetworkManager.Singleton.ShutdownInProgress);

        Scene onlineMenu = SceneManager.GetSceneByName("3_OnlineMainMenu");
        if (onlineMenu.IsValid() && onlineMenu.isLoaded)
        {
            Debug.Log("[Client] Descargando OnlineMenu");
            yield return SceneManager.UnloadSceneAsync(onlineMenu);
        }

        Scene lobby = SceneManager.GetSceneByName("4_LobbyMenu");
        if (lobby.IsValid() && lobby.isLoaded)
            SceneManager.SetActiveScene(lobby);

        GameManager.Instance.ChangeGameState(EGameState.Lobby);
        GameServiceLocator.GetService<LoadScreen_System>().ToggleLoaderScreen(false);
    }  

    private IEnumerator LoadLobbyNextFrame()
    {
        yield return new WaitUntil(() => 
        !NetworkManager.Singleton.ShutdownInProgress &&
        NetworkManager.Singleton.IsListening);
        GameManager.Instance.RequestOnlineLobbyScene();
    }


    private void MethodOnClientDisconnect(ulong playerId)
    {
    
        Debug.Log($"[Connection] Desconectado: {playerId}");
        if (playerId == NetworkManager.Singleton.LocalClientId)
        {
            GameManager.Instance.ReturnToOnlineMenuAfterSession();
        }
    
    }
}
