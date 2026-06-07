using UnityEngine;
using Unity.Netcode;

public class OnlineMenuManager : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField playerNameInputField;

    private void Start()
    {
        playerNameInputField.text = "Player" + Random.Range(1000, 9999);
    }

    public void Out_StartHost()
    {
        OnlineGameData.Singleton.PlayerName = playerNameInputField.text;
        NetworkManager.Singleton.StartHost();
    }

    public void Out_StartClient()
    {
        OnlineGameData.Singleton.PlayerName = playerNameInputField.text;
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
