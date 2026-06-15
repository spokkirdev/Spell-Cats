using UnityEngine;
using Unity.Netcode;

public class NetworkCat : NetworkBehaviour
{

    public void Collect()
    {
        if (!IsServer) return;

        // Call the CollectCat method on the NetworkCatSpawner
        NetworkCatSpawner catSpawner = GameServiceLocator.GetService<NetworkCatSpawner>();
        if (catSpawner != null)
        {
            catSpawner.CollectCat();
        }

        // Destroy the cat object across the network
        GetComponent<NetworkObject>().Despawn();
    }
}
