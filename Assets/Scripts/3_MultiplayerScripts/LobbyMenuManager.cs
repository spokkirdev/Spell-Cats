using UnityEngine;
using Unity.Netcode;

public class LobbyMenuManager : MonoBehaviour
{

   public void Out_ShutDownOnline()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public void Out_PlayerReady()
    {
        OnlineGameData.Singleton.UpdatePlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }
}
