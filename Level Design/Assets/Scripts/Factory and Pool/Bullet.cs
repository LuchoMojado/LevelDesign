using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float count;
    public float speed;
    ObjectPool<Bullet> _objectPool;
    
    // no se puede crear en el start el object pool porque cada bala haria uno nuevo 

    public void Initialize(ObjectPool<Bullet> op)
    {
        _objectPool = op;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        count += Time.deltaTime;
        if(count > 4)
        {
            _objectPool.RefillStock(this);
        }
    }

    public static void TurnOff(Bullet x)
    {
        x.gameObject.SetActive(false);
        x.count = 0;
    }
    public static void TurnOn()
    {

    }
}
