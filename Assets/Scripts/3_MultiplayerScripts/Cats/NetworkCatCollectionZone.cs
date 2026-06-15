using UnityEngine;
using Unity.Netcode;

public class NetworkCatCollectionZone : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Cat"))
        {
            NetworkCat cat = other.GetComponent<NetworkCat>();
            if (cat != null)
            {
                cat.Collect();
            }
        }
    }
}
