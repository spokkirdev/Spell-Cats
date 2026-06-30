using UnityEngine;
using Unity.Netcode;

public class NetworkCat : NetworkBehaviour
{

    [SerializeField] private int pointsToAdd = 50;
    public void Collect()
    {
        if (!IsServer) return;

        // Call the CollectCat method on the NetworkCatSpawner
        NetworkCatSpawner catSpawner = GameServiceLocator.GetService<NetworkCatSpawner>();
        if (catSpawner != null)
        {
            catSpawner.CollectCat();
        }

        // Add points to the player's score
        GameServiceLocator.GetService<NetworkPoints>().AddPoints(pointsToAdd);

        // Destroy the cat object across the network
        GetComponent<NetworkObject>().Despawn();
    }

    public void Respawn()
    {
        if (!IsServer) return;

        // Call the RespawnCat method on the NetworkCatSpawner
        NetworkCatSpawner catSpawner = GameServiceLocator.GetService<NetworkCatSpawner>();
        if (catSpawner != null)
        {
            catSpawner.RespawnCat();
        }
        GetComponent<NetworkObject>().Despawn();
    }
}
