using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class OnlineMenuManager : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private TMPro.TMP_InputField playerNameInputField;

    [SerializeField] private TMPro.TMP_InputField joinCodeInput;

    [SerializeField] private  GameObject joinGameButton;
      [SerializeField] private  GameObject joinGameButton2;
    [SerializeField] private GameObject hostGameButton;
    [SerializeField] private GameObject RoomCodeText;
    [SerializeField] private GameObject RoomCodeField;
    [SerializeField] private TMPro.TMP_Text JoinGame_BackText;


    [SerializeField] private TMPro.TMP_Text joinCodeDisplay;

    private OnlineGameData _onlineGameData;
    private RelayManager _relayManager;

    private void Start()
    {
        _onlineGameData = GameServiceLocator.GetService<OnlineGameData>();
        _relayManager = GameServiceLocator.GetService<RelayManager>();
        playerNameInputField.text = "Player" + Random.Range(1000, 9999);
    }

    public async void Out_StartHost()
    {
        _onlineGameData.PlayerName = playerNameInputField.text;
        try
        {
            string joinCode = await _relayManager.CreateRoomAsync(4);
            joinCodeDisplay.text = $"Join Code: {joinCode}";
        }
        catch
        {
            joinCodeDisplay.text = "Error al crear sala";
        }
    }

    public async void Out_StartClient()
    {
        _onlineGameData.PlayerName = playerNameInputField.text;

        bool joined = await _relayManager.JoinRoomAsync(joinCodeInput.text);
        if (!joined)
            joinCodeDisplay.text = "No se pudo unir a la sala";
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

    public void Out_ToggleJoinGameButton()
    {
        if (JoinGame_BackText.text == "Join Game")
        {
            JoinGame_BackText.text = "Back";
        }
        else
        {
            JoinGame_BackText.text = "Join Game";
        }
        joinGameButton2.SetActive(!joinGameButton2.activeSelf);
        hostGameButton.SetActive(!hostGameButton.activeSelf);
        RoomCodeText.SetActive(!RoomCodeText.activeSelf);
        RoomCodeField.SetActive(!RoomCodeField.activeSelf);
        
    }

        
}
