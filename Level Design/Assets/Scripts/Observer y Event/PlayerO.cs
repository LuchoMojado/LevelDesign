using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerO : MonoBehaviour, IObservable
{
    public List<IObserver> listObservers = new();

    public void Subscribe(IObserver x)
    {
        listObservers.Add(x);
    }

    public void Unsubscribe(IObserver x)
    {
        if(listObservers.Contains(x))
            listObservers.Remove(x);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ACA en vez de eso es poner lo de si estoy corriendo o haciendo mucho ruido te persigo
        if(Input.GetKeyDown(KeyCode.K))
        {
            foreach(var item in listObservers)
            {
                item.Notify(transform.position);
            }
        }
    }
}
