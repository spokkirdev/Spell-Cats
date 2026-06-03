using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ConnectionCallBackManager : MonoBehaviour
{
    private static ConnectionCallBackManager _singleton;
    public static ConnectionCallBackManager Singleton => _singleton;

    private void Awake()
    {
        if(_singleton != null && _singleton != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void Start()
    {
        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientStarted += MethodOnClientStarted;
            NetworkManager.Singleton.OnClientStopped += MethodOnClientStopped;
            NetworkManager.Singleton.OnClientConnectedCallback += MethodOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += MethodOnClientDisconnect;
        }
    }

    public void OnDestroy()
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
        SceneManager.LoadScene("3_OnlineMainMenu"); 
    }
    

    private void MethodOnClientConnected(ulong playerId)
    {
        if(NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("4_LobbyMenu", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        Debug.Log("Client Connected: " + playerId);
    }

    private void MethodOnClientDisconnect(ulong playerId)
    {
        Debug.Log("Client Disconnected: " + playerId);
    }
}
