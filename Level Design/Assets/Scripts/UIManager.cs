using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] GameObject _pause, _options, _lang, _audio, _back, _sensitivity;
    [SerializeField] Player _player;
    [SerializeField] Slider _sensSlider;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        _sensSlider.value = _player.movement._mouseSensitivity;
    }
    public void SetPauseMenu(bool pause)
    {
        if (pause)
        {
            //Debug.Log("awg");
            _pause.SetActive(true);
        }
        else
        {
            //Debug.Log("guwa");
            _options.SetActive(false);
            _pause.SetActive(false);
            _lang.SetActive(false);
            _audio.SetActive(false);
            _back.SetActive(false);
        }
    }
    public void SetOptionsMenu()
    {
        if (_options.activeInHierarchy)
        {
            _lang.SetActive(false);
            _audio.SetActive(false);
            _options.SetActive(false);
            _back.SetActive(false);
            _pause.SetActive(true);
        }
        else
        {
            _lang.SetActive(false);
            _audio.SetActive(false);
            _pause.SetActive(false);
            _options.SetActive(true);
            _back.SetActive(true);
        }
    }
    public void SetLanguageMenu()
    {
        _pause.SetActive(false);
        _lang.SetActive(true);
        _options.SetActive(false);
        _audio.SetActive(false);
        _back.SetActive(true);
    }
    public void SetAudioMenu()
    {
        _pause.SetActive(false);
        _lang.SetActive(false);
        _options.SetActive(false);
        _audio.SetActive(true);
        _back.SetActive(true);
    }
    public void ChangeSensitivity()
    {
        _player.movement._mouseSensitivity = _sensSlider.value;
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
