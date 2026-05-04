using UnityEngine;

[CreateAssetMenu(fileName = "New Levitate Spell", menuName = "Spells/Levitate")]
public class LevitateSpell : Spell
{
    public override void Cast(Transform caster)
    {
        Debug.Log($"{spellName} cast by {caster.name}");
        caster.GetComponent<FPSMovement>().StartLevitation();     
    }

    public override void Stop(Transform caster)
    {
        Debug.Log($"{spellName} stopped by {caster.name}");
        caster.GetComponent<FPSMovement>().StopLevitation();
    }
}
