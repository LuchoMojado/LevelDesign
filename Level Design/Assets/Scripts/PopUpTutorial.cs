using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.SceneManagement;
using TMPro;

public class PopUpTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject Panel;
    [SerializeField] TMP_Text _texto;
    string _textoTutorial;
    [SerializeField] string ID;
    void Start()
    {
        LocalizationManager.instance.EventTranslate += Translate;
        Translate();
    }

    private void OnTriggerEnter(Collider other)
    {
        _texto.SetText(_textoTutorial);
        //Panel.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        _texto.SetText("");
        //Panel.SetActive(false);
    }

    void Translate()
    {
        _textoTutorial = (LocalizationManager.instance.GetTranslate(ID));
    }
}
