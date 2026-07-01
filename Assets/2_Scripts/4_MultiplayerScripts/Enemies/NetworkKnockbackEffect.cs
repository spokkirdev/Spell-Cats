using Unity.Netcode;
using UnityEngine;

public class NetworkKnockbackEffect: NetworkBehaviour, IAttackEffect
{
    [Header("Settings")]
    [SerializeField] private float knockbackForce = 10f;

    public int Priority => 5;

    public void Apply(Transform target, Transform attacker)
    {
        NetworkObject targetNetworkObject = target.GetComponentInParent<NetworkObject>();
        if (targetNetworkObject == null)
        {
            Debug.LogWarning($"No NetworkObject found on {target.name}");
            return;
        }

        // Dirección robusta
        Vector3 direction = target.position - attacker.position;
        direction.y = 0f;

        // Evitar vector cero
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = attacker.forward;
        }

        direction.Normalize();

        Debug.Log($"Applying knockback: dir={direction}, force={knockbackForce}");

        ApplyKnockbackRpc(targetNetworkObject.NetworkObjectId, direction, knockbackForce);
    }


    [Rpc(SendTo.Everyone, InvokePermission =RpcInvokePermission.Server)]
    private void ApplyKnockbackRpc(ulong targetNetworkId, Vector3 direction, float force)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkId, out NetworkObject targetNetObj))
        {
            Debug.LogWarning($"[KnockbackEffect] No se encontró NetworkObject con ID {targetNetworkId}");
            return;
        }
        if (NetworkManager.Singleton.LocalClientId != targetNetObj.OwnerClientId)
        {
            Debug.Log($"Received knockback RPC for NetworkObjectId {targetNetworkId}, but this client has id {NetworkManager.Singleton.LocalClientId}. Ignoring.");
            return;
        }
        Debug.Log($"Received knockback RPC: dir={direction}, force={force}");
        var movement = targetNetObj.GetComponentInChildren<NetworkFPSMovement>();
        movement?.ReceiveKnockback(direction, force);
    }
}
