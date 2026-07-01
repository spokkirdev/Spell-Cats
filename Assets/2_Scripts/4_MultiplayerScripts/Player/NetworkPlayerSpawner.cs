using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject networkPlayerPrefab;
    [SerializeField] private float spawnRadius = 2f;


    public override void OnNetworkSpawn()
    {
        GameServiceLocator.RegisterService<NetworkPlayerSpawner>(this);
        Debug.Log($"[NetworkPlayerSpawner] OnNetworkSpawn called for clientId: {NetworkManager.Singleton.LocalClientId}, isHost: {NetworkManager.Singleton.IsHost}");
        SpawnPlayerForClientRpc(NetworkManager.Singleton.LocalClientId);
    }

    public override void OnNetworkDespawn()
    {
        GameServiceLocator.UnregisterService<NetworkPlayerSpawner>();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SpawnPlayerForClientRpc(ulong clientId)
    {

        
        Vector3 spawnPosition = transform.position + new Vector3(
            Random.insideUnitCircle.x, 0f, Random.insideUnitCircle.y) * spawnRadius;
                
        NetworkObject newPlayer = Instantiate(networkPlayerPrefab, spawnPosition, transform.rotation);
        
        Scene spawnerScene = gameObject.scene;
        SceneManager.MoveGameObjectToScene(newPlayer.gameObject, spawnerScene);
        
        newPlayer.SpawnWithOwnership(clientId);
        
        
        newPlayer.DestroyWithScene = true;
    }

    public void SpawnPlayer(ulong clientId)
    {
        Vector3 spawnPosition = transform.position +
            new Vector3(Random.insideUnitCircle.x, 0f, Random.insideUnitCircle.y) * spawnRadius;

        NetworkObject newPlayer =
            Instantiate(networkPlayerPrefab, spawnPosition, transform.rotation);

        SceneManager.MoveGameObjectToScene(newPlayer.gameObject, gameObject.scene);

        newPlayer.SpawnWithOwnership(clientId);
        newPlayer.DestroyWithScene = true;
    }
}
