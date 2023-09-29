using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerF : MonoBehaviour
{
    //Pide las balas
    public Bullet bulletPrefab;
    public Factory<Bullet> factory;
    public Factory<Bullet> factory2;
    ObjectPool<Bullet> myPool;

    // Start is called before the first frame update
    void Start()
    {
        factory = new Factory<Bullet>(bulletPrefab);
        factory2 = new Factory<Bullet>(bulletPrefab);
        myPool = new ObjectPool<Bullet>(factory.GetObject,Bullet.TurnOff, Bullet.TurnOff,20);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Le pasa todo al object pool para que haga las balas 
            var x = myPool.Get();
            x.Initialize(myPool);
            x.transform.position = transform.position;
            x.transform.forward = transform.position;
        }
    }
}
