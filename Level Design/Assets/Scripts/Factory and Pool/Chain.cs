using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public float count;
    public float speed;
    ObjectPool<Chain> _objectPool;
    //public Transform tr;
    public ConfigurableJoint cf;

    // no se puede crear en el start el object pool porque cada bala haria uno nuevo 

    private void Awake()
    {
        cf = this.GetComponent<ConfigurableJoint>();
    }

    public void Initialize(ObjectPool<Chain> op)
    {
        _objectPool = op;
    }

    // Update is called once per frame
    void Update()
    {
        //cf.anchor = tr.position;
        //transform.position += transform.forward * speed * Time.deltaTime;
        //count += Time.deltaTime;
        /*if (count > 4)
        {
            _objectPool.RefillStock(this);
        }*/
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
