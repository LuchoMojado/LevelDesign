using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MejorEnemyO : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Subscribe("Call", Chase);
    }


    //object puede ser lo que sea literal
    void Chase(params object[] pos)
    {
        //Aclara que pos es un vector3
        Debug.Log("Lo persigo" + (Vector3)pos[0]);
    }
}
