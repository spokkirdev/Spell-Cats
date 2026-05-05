using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class RadialMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image icon;


    
    public Action<RadialMenuEntry> OnEntryClicked;

    RectTransform _rectTransform;

    void Start()
    {
        _rectTransform = icon.GetComponent<RectTransform>();
    }

    public void SetLabel(string text)
    {
        if (text != null)
        {
            label.text = text;
        }
    }

    public void SetIcon(Sprite sprite)
    {
        if (sprite != null)
        {
            icon.sprite = sprite;
        }
    }

    public Sprite GetIcon()
    {
        return icon.sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on: " + label.text);
        OnEntryClicked?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _rectTransform.DOComplete();
        _rectTransform.DOScale(Vector3.one * 1.5f, 0.3f).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rectTransform.DOComplete();
        _rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad);
    }
}