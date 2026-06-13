using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyMenuManager : MonoBehaviour
{
    [SerializeField] private Button newGameButton;

    private OnlineGameData _onlineGameData;


    private void Awake()
    {
        _onlineGameData = GameServiceLocator.GetService<OnlineGameData>();
        newGameButton.interactable = NetworkManager.Singleton.IsHost;

    }


   public void Out_ShutDownOnline()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public void Out_PlayerReady()
    {
        _onlineGameData.UpdatePlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void Out_NewGame()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        GameManager.Instance.RequestOnlineGameScene();
    }
}
