using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class PopUpTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Panel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
            Panel.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        Panel.SetActive(false);
    }
}
