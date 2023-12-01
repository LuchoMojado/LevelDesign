using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonTranslate : MonoBehaviour
{
    public string ID;
    public TextMeshProUGUI textUI;

    void Start()
    {
        LocalizationManager.instance.EventTranslate += Translate;
        Translate();
    }

    void Translate()
    {
        textUI.text = LocalizationManager.instance.GetTranslate(ID);
    }
}
