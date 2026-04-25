using UnityEngine;

public class KnockbackEffect : AttackEffect
{
    [Header("Settings")]
    [SerializeField] private float knockbackForce = 10f;

    public override void Apply(Transform target, Transform attacker)
    {
        Debug.Log($"Applying KnockbackEffect to {target.name} from {attacker.name}");
        CharacterController controller = target.GetComponent<CharacterController>();
        if (controller != null)        
        {
            Debug.Log($"Applying {knockbackForce} knockback to {target.name} from {attacker.name}");
            controller.GetComponent<FPSMovement>()?.ReceiveKnockback((target.position - attacker.position).normalized, knockbackForce);
        }
    }
}
