using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportantObjects : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position,GameManager.gameManager.player.transform.position) <= 2f)
        {
            GameManager.gameManager.takeObject();
            Destroy(this.gameObject);
        }
    }
}
