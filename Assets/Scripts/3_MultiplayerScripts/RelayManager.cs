using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using System;


public class RelayManager : MonoBehaviour
{   

    [Header("UI References")]
    [SerializeField] private TMPro.TMP_InputField playJoinCodeInputField;
    [SerializeField] private TMPro.TMP_InputField playerNameInputField;

    [Header("Settings")]
    [SerializeField] private int maxConnections = 4;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        NetworkManager.Singleton.OnClientStopped += ResetJoinCode;

    }

    private void ResetJoinCode(bool obj)
    {
        JoinCode.Singleton.joinCodeText.text = "Join Code: ";
    }


    public async void Out_CreateRoom()
    {
        string joinCode = await StartHostWitchRelay(maxConnections);
        JoinCode.Singleton.joinCodeText.text = "Join Code: " + joinCode;
        Debug.Log($"Created room with join code: {joinCode}");
    }

    private async Task<string> StartHostWitchRelay(int maxConnections = 4)
    {
        Allocation allocation;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            
        }catch
        {
            Debug.LogError("Relay create allocation failed");
            throw;
        }

        if (allocation != null)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join code: {joinCode}");
            return NetworkManager.Singleton.StartHost() ? joinCode : null;
        } 
        else
        {
            Debug.LogError("Allocation is null");
            return "Error";
        }

    }

    public async void Out_JoinRoom()
    {
        await JoinHostWithRelay(playJoinCodeInputField.text);
    }

    private async Task<bool> JoinHostWithRelay(string joinCode)
    {
        if (joinCode == null || joinCode.Length == 0)
        {
            Debug.LogError("Join code is null or empty");
            return false;
        }

        JoinAllocation joinAllocation;
        try
        {
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Relay join allocation failed");
            throw;
        }

        if (joinAllocation != null)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, "dtls"));
            return NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.LogError("Join allocation is null");
            return false;
        }
    }
}
