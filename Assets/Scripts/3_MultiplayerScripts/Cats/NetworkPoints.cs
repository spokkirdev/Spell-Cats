using Unity.Netcode;
using UnityEngine;

public class NetworkPoints : NetworkBehaviour
{
    private NetworkVariable<int> _points = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public int Points => _points.Value;

    public void AddPoints(int amount)
    {
        if (IsServer)
        {
            _points.Value += amount;
        }
    }

    public void ResetPoints()
    {
        if (IsServer)
        {
            _points.Value = 0;
        }
    }
}
