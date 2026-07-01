using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SpawnData
{
    public NetworkObject enemyPrefab;
    public Transform spawnPoint;
    public GameObject patrolPointsParent; 
}

public class NetworkEnemySpawner : NetworkBehaviour
{
    [SerializeField] private SpawnData[] spawnDatas;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        Scene spawnerScene = gameObject.scene;

        foreach (var spawnData in spawnDatas)
        {
            if (spawnData.enemyPrefab == null || spawnData.spawnPoint == null)
            {
                Debug.LogWarning("SpawnData is not properly set up. Skipping this spawn.");
                continue;
            }
            NetworkObject enemyInstance = Instantiate(spawnData.enemyPrefab, spawnData.spawnPoint.position, spawnData.spawnPoint.rotation);
            SceneManager.MoveGameObjectToScene(enemyInstance.gameObject, spawnerScene);
            if (spawnData.patrolPointsParent != null)
            {
                var patrolling = enemyInstance.GetComponent<NetworkPatrollingEnemy>();
                patrolling?.SetPatrolPointsParent(spawnData.patrolPointsParent);
            }
            enemyInstance.DestroyWithScene = true;
            enemyInstance.Spawn();
        }
    }
}
