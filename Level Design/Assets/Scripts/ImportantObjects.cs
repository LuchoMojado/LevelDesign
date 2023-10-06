using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportantObjects : MonoBehaviour
{
    ObjectPool<ImportantObjects> _objectPool;
    public void Initialize(ObjectPool<ImportantObjects> op)
    {
        _objectPool = op;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position,GameManager.gameManager.player.transform.position) <= 2f)
        {
            GameManager.gameManager.takeObject();
            _objectPool.RefillStock(this);
        }
    }
    public static void TurnOff(ImportantObjects x)
    {
        x.gameObject.SetActive(false);
    }
    public static void TurnOn(ImportantObjects x)
    {
        x.gameObject.SetActive(true);
    }
}
