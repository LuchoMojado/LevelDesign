using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportantObjects : Rewind
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
            GameManager.gameManager.checkPointPos = GameManager.gameManager.player.transform.position;
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

    public override void Save()
    {
        _mementoState.Rec(transform.position,transform.rotation);
    }

    public override void Load()
    {
        if(_mementoState.IsRemember())
        {
            //Pongo en el array la pos que se donde lo puse lo que quiero
            transform.position = (Vector3)_mementoState.data[0];
            transform.rotation = (Quaternion)_mementoState.data[1];
        }
    }
}
