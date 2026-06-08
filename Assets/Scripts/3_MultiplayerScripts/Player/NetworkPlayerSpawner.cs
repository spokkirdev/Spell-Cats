using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject networkPlayerPrefab;
    [SerializeField] private float spawnRadius = 2f;

    public override void OnNetworkSpawn()
    {
        SpawnPlayerForClientRpc(NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server,InvokePermission = RpcInvokePermission.Everyone)]
    private void SpawnPlayerForClientRpc(ulong clientId)
    {
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        NetworkObject newPlayer = Instantiate(networkPlayerPrefab, spawnPosition, transform.rotation);
        newPlayer.SpawnWithOwnership(clientId);
        newPlayer.DestroyWithScene = true;
    }
}
