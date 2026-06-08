using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyMenuManager : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private string newGameSceneName = "OnlineGameScene";

    private void Start()
    {
        newGameButton.interactable = NetworkManager.Singleton.IsHost;
    }

   public void Out_ShutDownOnline()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public void Out_PlayerReady()
    {
        OnlineGameData.Singleton.UpdatePlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void Out_NewGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(newGameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
