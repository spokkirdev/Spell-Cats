using UnityEngine;
using System.Linq;

/*
* MeleeAttack is a specific type of Attack that represents close-range combat. It checks if the target is within a certain range and applies damage or effects accordingly.
* It also manages cooldowns to prevent the enemy from attacking too frequently.
*/
public class MeleeAttack : Attack
{

    [Header("Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float cooldownTime = 1.0f;

    private float _lastAttackTime;
    private IAttackEffect[] _attackEffects;

    private void Awake()
    {
        _attackEffects = GetComponents<IAttackEffect>().OrderBy(e => e.Priority).ToArray();
    }

    /*
    * PerformAttack applies all the effects of the attack to the target. It also updates the last attack time to manage cooldowns.
    */
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
