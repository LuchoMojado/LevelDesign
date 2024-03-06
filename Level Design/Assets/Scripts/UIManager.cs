using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] GameObject _pause, _options, _lang;

    private void Awake()
    {
        instance = this;
    }

    public void SetPauseMenu(bool pause)
    {
        if (pause)
        {
            Debug.Log("awg");
            _pause.SetActive(true);
        }
        else
        {
            Debug.Log("guwa");
            _pause.SetActive(false);
            _options.SetActive(false);
        }
    }
    public void Options()
    {
        _options.SetActive(true);
    }
    public void SetSpanish()
    {
        LocalizationManager.instance.ChangeLang(SystemLanguage.Spanish);
    }
    public void SetEnglish()
    {
        LocalizationManager.instance.ChangeLang(SystemLanguage.English);
    }
}
