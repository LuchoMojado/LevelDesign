using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider SFXSlider;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("volume", 1);
            PlayerPrefs.SetFloat("SFX", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFX");
    }
    private void Save()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        PlayerPrefs.SetFloat("SFX", SFXSlider.value);
    }
}
