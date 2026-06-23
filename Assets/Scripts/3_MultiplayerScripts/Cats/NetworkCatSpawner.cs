using Unity.Netcode;
using UnityEngine;

public class NetworkCatSpawner : NetworkBehaviour
{
    [Header("Cat Spawner Settings")]
    [SerializeField] private GameObject catPrefab;    
    [SerializeField] private GameObject catSpawnPointParent;  
    [SerializeField] private int maxCats = 2;



    private int _currentCatCount = 0;

    private Transform[] _catSpawnPoints;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        if (GameServiceLocator.TryGetService<NetworkCatSpawner>())
        {
            GameServiceLocator.UnregisterService<NetworkCatSpawner>();
        }
        GameServiceLocator.RegisterService<NetworkCatSpawner>(this);
        _catSpawnPoints = catSpawnPointParent.GetComponentsInChildren<Transform>();

        for (int i = 0; i < maxCats; i++)
        {
            SpawnCat();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (GameServiceLocator.TryGetService<NetworkCatSpawner>())
        {
            GameServiceLocator.UnregisterService<NetworkCatSpawner>();
        }
    }

    private void SpawnCat()
    {
        if (!IsServer) return;

        if (_currentCatCount >= maxCats)
        {
            Debug.Log("Max cat limit reached. Cannot spawn more cats.");
            return;
        }

        int randomIndex = Random.Range(0, _catSpawnPoints.Length);
        Transform spawnPoint = _catSpawnPoints[randomIndex];

        GameObject newCat = Instantiate(catPrefab, spawnPoint.position, spawnPoint.rotation);
        newCat.GetComponent<NetworkObject>().Spawn();

        _currentCatCount++;
    }

    public void CollectCat()
    {
        if (!IsServer) return;

        _currentCatCount--;

        //AÑADIR LOGICA DE PUNTOS AQUI

        SpawnCat();
    
    }
}
