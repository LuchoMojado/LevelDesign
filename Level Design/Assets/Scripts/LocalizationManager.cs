using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;
    public SystemLanguage language;
    public DataLocalization[] data;
    Dictionary<SystemLanguage, Dictionary<string, string>> _translate = new Dictionary<SystemLanguage, Dictionary<string, string>>();

    public event Action EventTranslate;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            _translate = LanguageU.LoadTranslate(data);
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeLang(SystemLanguage.Spanish);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeLang(SystemLanguage.English);
    }

    public void ChangeLang(SystemLanguage newLang)
    {
        if(language != newLang)
        {
            language = newLang;

            EventTranslate?.Invoke();
        }  
    }

    public string GetTranslate(string id)
    {
        if (!_translate.ContainsKey(language))
            return "No lang";

        if (!_translate[language].ContainsKey(id))
            return "No ID";

        return _translate[language][id];
    }
}
