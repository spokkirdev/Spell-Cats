using UnityEngine;

public abstract class Spell : ScriptableObject
{
    public string spellName;
    public Sprite spellIcon;
    public float cooldownTime;
    public float effectDuration;


    public abstract void Cast(Transform caster);

    public abstract void Stop(Transform caster);
}
