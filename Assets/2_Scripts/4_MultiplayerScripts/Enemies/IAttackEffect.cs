using UnityEngine;

public interface IAttackEffect
{
    int Priority { get; }
    void Apply(Transform target, Transform attacker);
}