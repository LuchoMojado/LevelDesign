using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportantObjects : MonoBehaviour
{
    ObjectPool<ImportantObjects> _objectPool;

    bool _loading;
    public void Initialize(ObjectPool<ImportantObjects> op)
    {
        _objectPool = op;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position,GameManager.instance.player.transform.position) <= 2f)
        {
            GameManager.instance.checkPointPos = GameManager.instance.player.transform.position;
            GameManager.instance.takeObject();
            _objectPool.RefillStock(this);
            GameManager.instance.player.Save();
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

    //public override void Save()
    //{
    //    if (_loading)
    //        return;
    //    _mementoState.Rec(transform.position,transform.rotation);
    //}
    //
    //public override void Load()
    //{
    //    //if(_mementoState.IsRemember())
    //    //{
    //    //    var data = _mementoState.Remember();
    //    //    //Pongo en el array la pos que se donde lo puse lo que quiero
    //    //    transform.position = (Vector3)data.parameters[0];
    //    //    transform.rotation = (Quaternion)data.parameters[1];
    //    //}
    //    if (_mementoState.IsRemember())
    //    {
    //        StartCoroutine(CoroutineLoad());
    //    }
    //}
    //
    //IEnumerator CoroutineLoad()
    //{
    //    var WaitForSeconds = new WaitForSeconds(0.01f);
    //    _loading = true;
    //    while (_mementoState.IsRemember())
    //    {
    //        var data = _mementoState.Remember();
    //        //_loading = true;
    //        //Pongo en el array la pos que se donde lo puse lo que quiero
    //        transform.position = (Vector3)data.parameters[0];
    //        transform.rotation = (Quaternion)data.parameters[1];
    //
    //        yield return WaitForSeconds;
    //    }
    //    _loading = false;
    //}
}
