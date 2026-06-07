using UnityEngine;
using Unity.Netcode;

public class ChatManager : NetworkBehaviour
{

    [Header("UI References")]
    [SerializeField] private TMPro.TMP_Text chatLog;
    [SerializeField] private TMPro.TMP_Text userListConnected;
    [SerializeField] private TMPro.TMP_InputField chatInputField;

    public override void OnNetworkSpawn()
    {
        UpdateUserList();
        OnlineGameData.Singleton.NewPlayerOnList += UpdateUserList;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        OnlineGameData.Singleton.NewPlayerOnList -= UpdateUserList;
    }

    private void UpdateUserList()
    {
        UpdateUserConnected(OnlineGameData.Singleton.OnlinePlayersConnected.ToArray());
    }


    public void Out_SendChatMessage()
    {
        SendMessageRpc(chatInputField.text, OnlineGameData.Singleton.PlayerName);
        chatInputField.text = "";
    }

    [Rpc(SendTo.Everyone, InvokePermission =RpcInvokePermission.Everyone)]
    private void SendMessageRpc(string message, string PlayerNameThatSendsMessage)
    {
        chatLog.text += "\n <color=red>" + PlayerNameThatSendsMessage + "</color>: " + message;
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
