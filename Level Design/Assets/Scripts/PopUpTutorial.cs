using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using TMPro;

public class PopUpTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject Panel;
    [SerializeField] TMP_Text _texto;
    [SerializeField] string _textoTutorial;
    void Start()
    {
        
    }

    void Update()
    {

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
}
