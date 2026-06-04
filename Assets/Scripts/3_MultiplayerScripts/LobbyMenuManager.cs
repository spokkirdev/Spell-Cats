using UnityEngine;
using Unity.Netcode;

public class LobbyMenuManager : MonoBehaviour
{

   public void Out_ShutDownOnline()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
