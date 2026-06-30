using UnityEngine;

public class LimitsManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameServiceLocator.GetService<NetworkHealthComponent>().RequestRespawnServerRpc();
        }
        
        if (other.CompareTag("Cat"))
        {
            other.GetComponent<NetworkCat>().Respawn();
        }
    }
}
