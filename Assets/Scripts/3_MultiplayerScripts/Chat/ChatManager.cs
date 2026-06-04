using UnityEngine;
using Unity.Netcode;

public class ChatManager : NetworkBehaviour
{

    [Header("UI References")]
    [SerializeField] private TMPro.TMP_Text chatLog;
    [SerializeField] private TMPro.TMP_InputField chatInputField;


    public void Out_SendChatMessage()
    {
        SendMessageRpc(chatInputField.text);
        chatInputField.text = "";
    }

    [Rpc(SendTo.Everyone, InvokePermission =RpcInvokePermission.Everyone)]
    private void SendMessageRpc(string message)
    {
        chatLog.text += "\n" + message;
    }
}
