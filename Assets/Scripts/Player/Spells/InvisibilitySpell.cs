using UnityEngine;

[CreateAssetMenu(fileName = "New Invisibility Spell", menuName = "Spells/Invisibility")]
public class InvisibilitySpell : Spell
{
    public override void Cast(Transform caster)
    {
        Debug.Log($"{spellName} cast by {caster.name}");
        caster.GetComponent<FPSMovement>().StartInvisibility();
    }

    public override void Stop(Transform caster)
    {
        Debug.Log($"{spellName} stopped by {caster.name}");
        caster.GetComponent<FPSMovement>().StopInvisibility();
    }
}
