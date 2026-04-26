using UnityEngine;
using System.Linq;

public class MeleeAttack : Attack
{

    [Header("Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float cooldownTime = 1.0f;

    private float _lastAttackTime;
    private AttackEffect[] _attackEffects;

    private void Awake()
    {
        _attackEffects = GetComponents<AttackEffect>().OrderBy(e => e.Priority).ToArray();
    }

    public override void PerformAttack(Transform target)
    {
        _lastAttackTime = Time.time;

        foreach (var effect in _attackEffects)
        {
            effect.Apply(target, transform);
        }
    }


    public override bool CanAttack(Transform target)
    {
        if (Time.time - _lastAttackTime < cooldownTime)
        {
            return false;
        }
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= attackRange;
    }
}
