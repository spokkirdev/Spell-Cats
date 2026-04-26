using UnityEngine;

public class KnockbackEffect : AttackEffect
{
    [Header("Settings")]
    [SerializeField] private float knockbackForce = 10f;

    public override void Apply(Transform target, Transform attacker)
    {
        var movement = target.GetComponentInParent<FPSMovement>();
        if (movement == null)
        {
            Debug.LogWarning($"No FPSMovement found on {target.name}");
            return;
        }

        // Dirección robusta
        Vector3 direction = target.position - attacker.position;
        direction.y = 0f;

        // Evitar vector cero
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = attacker.forward;
        }

        direction.Normalize();

        Debug.Log($"Applying knockback: dir={direction}, force={knockbackForce}");

        movement.ReceiveKnockback(direction, knockbackForce);
    }
}