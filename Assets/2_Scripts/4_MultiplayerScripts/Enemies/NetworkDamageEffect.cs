using Unity.Netcode;
using UnityEngine;

public class NetworkDamageEffect : NetworkBehaviour, IAttackEffect
{
    [SerializeField] private float damageAmount = 10;
    public int Priority => 10;

    public void Apply(Transform target, Transform attacker)
    {
        var health = target.GetComponentInParent<NetworkHealthComponent>();
        if (health == null)
        {
            Debug.LogWarning($"[DamageEffect] No se encontró NetworkHealthComponent en {target.name}");
            return;
        }

        health.TakeDamage(damageAmount);
    }

    

}
