using UnityEngine;

public class DamageEffect : AttackEffect
{
    [Header("Settings")]
    [SerializeField] private float damageAmount = 10f;

    public override void Apply(Transform target, Transform attacker)
    {
        Debug.Log($"Applying {damageAmount} damage to {target.name} from {attacker.name}");
    }
}
