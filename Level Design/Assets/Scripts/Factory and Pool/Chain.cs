using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public float count;
    public float speed;
    ObjectPool<Chain> _objectPool;

    // no se puede crear en el start el object pool porque cada bala haria uno nuevo 

    public void Initialize(ObjectPool<Chain> op)
    {
        _objectPool = op;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += transform.forward * speed * Time.deltaTime;
        //count += Time.deltaTime;
        /*if (count > 4)
        {
            _objectPool.RefillStock(this);
        }*/
        if(Vector3.Distance(GameManager.gameManager.player.transform.position, this.transform.position) <= 2f)
        {
            //_objectPool.RefillStock(this);
        }
    }

    public static void TurnOff(Chain x)
    {
        x.gameObject.SetActive(false);
        x.count = 0;
    }
    public static void TurnOn(Chain x)
    {
        x.gameObject.SetActive(true);
    }
    public void Refil(Chain x)
    {
        _objectPool.RefillStock(this);
    }
}
