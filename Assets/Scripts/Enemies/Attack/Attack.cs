using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    public abstract void PerformAttack(Transform target);
    
    public abstract bool CanAttack(Transform target);
}
