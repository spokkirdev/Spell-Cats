using Unity.Netcode;
using UnityEngine;

public class NetworkPoints : NetworkBehaviour
{
    public NetworkVariable<int> Points = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        GameServiceLocator.RegisterService<NetworkPoints>(this);
    }

    public override void OnNetworkDespawn()
    {
        GameServiceLocator.UnregisterService<NetworkPoints>();
    }

    public void AddPoints(int amount)
    {
        if (IsServer)
        {
            Points.Value += amount;
        }
    }

    public void ResetPoints()
    {
        if (IsServer)
        {
            Points.Value = 0;
        }
    }
}
