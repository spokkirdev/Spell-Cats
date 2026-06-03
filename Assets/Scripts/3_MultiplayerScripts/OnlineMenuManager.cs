using UnityEngine;
using Unity.Netcode;

public class OnlineMenuManager : MonoBehaviour
{
    public void Out_StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Out_StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Out_ShutDownOnline()
    {
        NetworkManager.Singleton.Shutdown();
    }
    public void Out_QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
