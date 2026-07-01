using System;
using TMPro;
using UnityEngine;

public class JoinCode : MonoBehaviour
{

    [SerializeField] private TMP_Text joinCodeDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        String joinCode = GameServiceLocator.GetService<RelayManager>().JoinCode;
        if (!string.IsNullOrEmpty(joinCode))
            joinCodeDisplay.text = $"{joinCode}";
        else
            joinCodeDisplay.text = "Error at creating room";
    }

}
