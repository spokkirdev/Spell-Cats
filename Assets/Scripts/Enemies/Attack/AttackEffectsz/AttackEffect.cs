using UnityEngine;

public abstract class AttackEffect : MonoBehaviour
{
    [SerializeField] private int priority = 0;
    public int Priority => priority;
    
    public abstract void Apply(Transform target, Transform attacker);
}