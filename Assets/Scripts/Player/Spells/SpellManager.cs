using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class SpellManager : NetworkBehaviour
{
    [SerializeField] private Spell[] spells;
    [SerializeField] private Spell activeSpell;
    [SerializeField] private Transform spellCastPoint;

    [SerializeField] private InputActionReference castSpellAction;

    private bool _canCast = true;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        GameServiceLocator.RegisterService<SpellManager>(this);
        castSpellAction.action.performed += OnCastSpell;
        GameServiceLocator.GetService<UIManager>().UpdateActiveSpellUI();

    }


    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        GameServiceLocator.UnregisterService<SpellManager>();
        castSpellAction.action.performed -= OnCastSpell;
    }
    private void OnEnable()
    {
        if (!IsOwner) return;
        castSpellAction.action.performed += OnCastSpell;
    }

    private void OnDisable()
    {
        if (!IsOwner) return;
        castSpellAction.action.performed -= OnCastSpell;
    }

    private void Start()
    {
        if (!IsOwner) return;
        RadialMenu radialMenu = GameServiceLocator.GetService<RadialMenu>();
        if (radialMenu != null)
        {
            radialMenu.Initialize(this);
        }
    }


    public void CastActiveSpell()
    {
        if (activeSpell != null && _canCast)
        {
            float cooldownTime = activeSpell.cooldownTime;
            float effectDuration = activeSpell.effectDuration;
            Spell spell = activeSpell;
            activeSpell.Cast(spellCastPoint);
            StartCoroutine(EffectDurationCoroutine(effectDuration, cooldownTime, spellCastPoint, spell));
        }
    }

    private void OnCastSpell(InputAction.CallbackContext context)
    {
        CastActiveSpell();
    }

    private System.Collections.IEnumerator CooldownCoroutine(float cooldownTime)
    {
        _canCast = false;
        GameServiceLocator.GetService<UIManager>().SetSpellCooldownUI();
        yield return new WaitForSeconds(cooldownTime);
        GameServiceLocator.GetService<UIManager>().UpdateActiveSpellUI();
        _canCast = true;
    }

    private System.Collections.IEnumerator EffectDurationCoroutine(float duration,float cooldownTime, Transform caster, Spell spell)
    {
        _canCast = false;
        GameServiceLocator.GetService<UIManager>().CastSpellUI();
        yield return new WaitForSeconds(duration);
        spell.Stop(caster);
        StartCoroutine(CooldownCoroutine(cooldownTime));
    }

    public void SetActiveSpell(Spell newSpell)
    {
        activeSpell = newSpell;
    }

    public Spell GetActiveSpell()
    {
        return activeSpell;
    }

    
}
