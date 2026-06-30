using UnityEngine;
using Unity.Netcode;
using System;

public class ChatManager : NetworkBehaviour
{

    [Header("UI References")]
    [SerializeField] private TMPro.TMP_Text chatLog;
    [SerializeField] private TMPro.TMP_Text userListConnected;
    [SerializeField] private TMPro.TMP_InputField chatInputField;


    private String _playerColor = "#FF0000"; 

    private OnlineGameData _onlineGameData;


    public override void OnNetworkSpawn()
    {
        _onlineGameData = GameServiceLocator.GetService<OnlineGameData>();
        UpdateUserList();
        _onlineGameData.OnPlayersChanged += UpdateUserList;

        Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        _playerColor = "#" + ColorUtility.ToHtmlStringRGB(randomColor);

    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _onlineGameData.OnPlayersChanged -= UpdateUserList;
    }

    private void UpdateUserList()
    {
        UpdateUserConnected(_onlineGameData.OnlinePlayersConnected.ToArray());
    }


    public void Out_SendChatMessage()
    {
        SendMessageRpc(chatInputField.text, _onlineGameData.PlayerName);
        chatInputField.text = "";
    }

    [Rpc(SendTo.Everyone, InvokePermission =RpcInvokePermission.Everyone)]
    private void SendMessageRpc(string message, string PlayerNameThatSendsMessage)
    {
        chatLog.text += "\n <color=" + _playerColor + ">" + PlayerNameThatSendsMessage + "</color>: " + message;
    }

    public void UpdateUserConnected(OnlinePlayerData[] usersConnected)
    {
        userListConnected.text = "Users Connected: \n";
        foreach (OnlinePlayerData player in usersConnected)
        {
            userListConnected.text += player.PlayerName + " (" + player.ClientId + ") - " + 
                (player.isReady ? "Ready" : "Not Ready") + "\n";
        }
    }
}
