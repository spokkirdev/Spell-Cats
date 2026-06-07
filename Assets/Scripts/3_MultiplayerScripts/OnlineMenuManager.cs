using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class OnlineMenuManager : MonoBehaviour
{

    [SerializeField] private TMPro.TMP_InputField playerNameInputField;

    [SerializeField] private TMPro.TMP_InputField ipAddressInputField;
    [SerializeField] private TMPro.TMP_InputField portInputField;

    private void Start()
    {
        playerNameInputField.text = "Player" + Random.Range(1000, 9999);
    }

    public void Out_StartHost()
    {
        OnlineGameData.Singleton.PlayerName = playerNameInputField.text;
        if (SetConnectionData())
        {
            NetworkManager.Singleton.StartHost();
        }
    }

    public void Out_StartClient()
    {
        OnlineGameData.Singleton.PlayerName = playerNameInputField.text;
        if (SetConnectionData())
        {
            NetworkManager.Singleton.StartClient();
        }
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

    public bool SetConnectionData()
    {
        if (ushort.TryParse(portInputField.text, out ushort port))
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ipAddressInputField.text, port);
            return true;
        }
        else
        {
            Debug.LogError("Invalid port number.");
            return false;
        }

    }
        
}
