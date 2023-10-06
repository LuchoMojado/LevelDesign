using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MejorPlayerO : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            EventManager.Trigger("Call", transform.position);
        }
    }
}

//Call manda la posicion del player a los que ven, estos piden recibir un vector3 y es este, lo usan para ver si estan dentro de su rango, tmb un bool si esta haciendo ruido