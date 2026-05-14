using UnityEngine;

/*
* Base class for all enemy attacks. Defines the interface for performing an attack and checking if an attack can be performed.
* Specific attack types (e.g., MeleeAttack, RangedAttack) will inherit from this class and implement the abstract methods.
*/
public abstract class Attack : MonoBehaviour
{
    public abstract void PerformAttack(Transform target);
    
    public abstract bool CanAttack(Transform target);
}
