using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MejorEnemyO : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Subscribe("Call", Chase);
        Chase(1);
    }


    //object puede ser lo que sea literal
    void Chase(object pos)
    {
        Debug.Log("Lo persigo");
    }
}
