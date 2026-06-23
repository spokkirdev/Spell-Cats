using UnityEngine;
using Unity.Netcode;

public class NetworkTimer : NetworkBehaviour
{

    [SerializeField] private GameObject endGameCanvas;
    private NetworkVariable<double> _endTime =
        new NetworkVariable<double>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public bool IsTimerRunning => _endTime.Value > 0;

    public double EndTime => _endTime.Value;

    public override void OnNetworkSpawn()
    {
        GameServiceLocator.RegisterService<NetworkTimer>(this);
    }

    public override void OnNetworkDespawn()
    {
        GameServiceLocator.UnregisterService<NetworkTimer>();
    }

    private void Update()
    {
        if (!IsServer || !IsTimerRunning)
            return;

        double remainingTime =
            _endTime.Value - NetworkManager.Singleton.ServerTime.Time;

        if (remainingTime <= 0)
        {
            _endTime.Value = 0; 
            OnTimerFinishedClientRpc();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void StartTimerServerRpc(float duration)
    {
        _endTime.Value =
            NetworkManager.Singleton.ServerTime.Time + duration;
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopTimerServerRpc()
    {
        _endTime.Value = 0;
    }

    [ClientRpc]
    private void OnTimerFinishedClientRpc()
    {
        endGameCanvas.SetActive(true);
        Debug.Log("[NetworkTimer] Timer finished!");
    }

    public double GetRemainingTime()
    {
        if (!IsTimerRunning)
            return 0;

        return _endTime.Value - NetworkManager.Singleton.ServerTime.Time;
    }
}