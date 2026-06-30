using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using System;
using UnityEngine.SceneManagement;
using System.Collections;


public class RelayManager : MonoBehaviour
{   



    [Header("Settings")]
    [SerializeField] private int maxConnections = 4;

    public String JoinCode { get; private set; } = string.Empty;
    

    private bool _initialized = false;

    private void Awake()
    {
        GameServiceLocator.RegisterService<RelayManager>(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<RelayManager>();
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            _initialized = true;
            Debug.Log("[Relay] Servicios inicializados");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Relay] Error inicializando: {e.Message}");
            throw;
        }
    }


    public async Task<string> CreateRoomAsync(int maxPlayers)
    {
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.LogWarning("NetworkManager aún activo, forzando shutdown...");
            NetworkManager.Singleton.Shutdown();
            await Task.Delay(300);
        }
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode   = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton
                .GetComponent<UnityTransport>()
                .SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            if (!NetworkManager.Singleton.StartHost())
                throw new Exception("StartHost() devolvió false");

            NetworkManager.Singleton.SceneManager
                .SetClientSynchronizationMode(LoadSceneMode.Additive);

            Debug.Log($"[Relay] Sala creada. Join code: {joinCode}");
            JoinCode = joinCode;
            return joinCode;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Relay] Error creando sala: {e.Message}");
            JoinCode = string.Empty;
            throw;
        }
    }

   public async Task<bool> JoinRoomAsync(string joinCode)
    {
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton
                .GetComponent<UnityTransport>()
                .SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, "dtls"));

            bool started = NetworkManager.Singleton.StartClient();
            if (!started) return false;

            NetworkManager.Singleton.SceneManager
                .SetClientSynchronizationMode(LoadSceneMode.Additive);

            JoinCode = joinCode;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Relay] Error uniéndose: {e.Message}");
            JoinCode = string.Empty;
            return false;
        }
    }


    public IEnumerator CloseRoomCoroutine()
{
    Debug.Log("[Relay] Cerrando sala...");

    if (NetworkManager.Singleton != null)
    {
        NetworkManager.Singleton.Shutdown();
    }

    yield return new WaitUntil(() =>
        NetworkManager.Singleton == null ||
        !NetworkManager.Singleton.IsListening);

    Debug.Log("[Relay] Shutdown completado");
}
}
