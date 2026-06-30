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
            activeSpell.Cast(spellCastPoint);
            StartCoroutine(EffectDurationCoroutine(activeSpell.effectDuration, spellCastPoint));
            StartCoroutine(CooldownCoroutine(activeSpell.cooldownTime));
        }
    }

    private void OnCastSpell(InputAction.CallbackContext context)
    {
        CastActiveSpell();
    }

    private System.Collections.IEnumerator CooldownCoroutine(float cooldownTime)
    {
        _canCast = false;
        yield return new WaitForSeconds(cooldownTime);
        _canCast = true;
    }

    private System.Collections.IEnumerator EffectDurationCoroutine(float duration, Transform caster)
    {
        yield return new WaitForSeconds(duration);
        activeSpell.Stop(caster);
    }

    public void SetActiveSpell(Spell newSpell)
    {
        activeSpell = newSpell;
    }

    
}
