using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class RadialMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private List<Spell> spells;
    [SerializeField] private SpellManager spellManager;


    private List<RadialMenuEntry> _entries;

    private void Awake()
    {
        GameServiceLocator.RegisterService<RadialMenu>(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<RadialMenu>();
    }

    public void Initialize(SpellManager spellManager)
    {
        this.spellManager = spellManager;
    
        _entries = new List<RadialMenuEntry>();
    }


    private void CreateEntry(Spell spell)
    {
        GameObject entryObj = Instantiate(entryPrefab, transform);
        RadialMenuEntry entry = entryObj.GetComponent<RadialMenuEntry>();
        if (entry != null)
        {
            entry.SetLabel(spell.spellName);
            entry.SetIcon(spell.spellIcon);
            entry.SetSpell(spell);
            entry.OnEntryClicked += HandleEntryClicked;
            _entries.Add(entry);
        }
        else
        {
            Debug.LogError("Entry prefab does not have a RadialMenuEntry component.");
            Destroy(entryObj);
        }
    }

    public void OpenMenu()
    {
        for (int i = 0; i < spells.Count; i++)
        {
            CreateEntry(spells[i]);
        }
        RearrangeEntries();
    }

    public void CloseMenu()
    {
        foreach (var entry in _entries)
        {
            RectTransform rectTransform = entry.GetComponent<RectTransform>();
            rectTransform.DOAnchorPos(Vector3.zero, 0.3f).SetEase(Ease.OutQuad).onComplete = 
                delegate() {
                    Destroy(entry.gameObject);
                };
        }
        _entries.Clear();
    }

    public bool ToggleMenu()
    {
        if (_entries.Count > 0)
        {
            CloseMenu();
            return false;
        }
        else
        {
            OpenMenu();
            return true;
        }
    }

    private void RearrangeEntries()
    {
        float angleStep = 360f / _entries.Count;
        for (int i = 0; i < _entries.Count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 100f; // Adjust radius as needed
            RectTransform rectTransform = _entries[i].GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.zero; // Start from zero scale for animation
            rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad).SetDelay(i * 0.05f); // Staggered animation
            rectTransform.DOAnchorPos(position, 0.3f).SetEase(Ease.OutQuad).SetDelay(i * 0.05f);
        }
    }

    private void HandleEntryClicked(RadialMenuEntry entry)
    {
        Debug.Log("Clicked from menu: " + entry);
        spellManager.SetActiveSpell(entry.GetSpell());
        GameServiceLocator.GetService<UIManager>().UpdateActiveSpellUI();
        
    }
}
