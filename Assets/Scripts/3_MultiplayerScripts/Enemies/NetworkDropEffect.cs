using Unity.Netcode;
using UnityEngine;

public class NetworkDropEffect : NetworkBehaviour, IAttackEffect
{
    public int Priority => 9;

    public void Apply(Transform target, Transform attacker)
    {
        NetworkObject targetNetworkObject = target.GetComponentInParent<NetworkObject>();
        if (targetNetworkObject == null)
        {
            Debug.LogWarning($"No NetworkObject found on {target.name}");
            return;
        }

        Debug.Log($"Applying DropEffect to {target.name} from {attacker.name}");

        ForceDropRpc(targetNetworkObject.NetworkObjectId);


    }

    [Rpc(SendTo.Everyone, InvokePermission =RpcInvokePermission.Server)]
    private void ForceDropRpc(ulong targetNetworkId)
    {
        

        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkId, out NetworkObject targetNetObj))
        {
            Debug.LogWarning($"[DropEffect] No se encontró NetworkObject con ID {targetNetworkId}");
            return;
        }
        if (NetworkManager.Singleton.LocalClientId != targetNetObj.OwnerClientId)
        {
            Debug.Log($"Received drop RPC for NetworkObjectId {targetNetworkId}, but this client has id {NetworkManager.Singleton.LocalClientId}. Ignoring.");
            return;
        }
        var playerPickUpDrop = targetNetObj.GetComponentInChildren<NetworkPlayerPickUpDrop>();
        playerPickUpDrop.ForceDrop();
        return;
    }
}
