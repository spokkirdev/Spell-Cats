using UnityEngine;

public abstract class AttackEffect : MonoBehaviour
{
    public abstract void Apply(Transform target, Transform attacker);
}