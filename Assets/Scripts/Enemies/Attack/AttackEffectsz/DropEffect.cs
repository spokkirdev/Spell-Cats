using UnityEngine;

public class DropEffect : AttackEffect
{
    public override void Apply(Transform target, Transform attacker)
    {
        var playerPickUpDrop = target.GetComponentInParent<PlayerPickUpDrop>();

        if (playerPickUpDrop == null)
            return;

        Debug.Log($"Applying DropEffect to {target.name} from {attacker.name}");

        bool success = playerPickUpDrop.ForceDrop();

        if (!success)
        {
            Debug.Log("DropEffect applied but player was not holding anything");
        }
    }
}