using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager 
{
    public delegate void MyEvent(Vector3 pos);

    private static Dictionary<string,MyEvent> _events = new Dictionary<string, MyEvent>();

    public static void Subscribe(string name,MyEvent method)
    {
        if(_events.ContainsKey(name))
        {
            //Me dice el nombre del metodo y yo lo agrego
            //Tembien puede decir ej quiero susbcribirme a Call y hago chase y otro puede decir yo tmb hago call pero hago test, por eso se suman method
            _events[name] += method;
        }
        else
        {
            _events.Add(name, method);
        }
    }
    public static void UnSubscribe(string name, MyEvent method)
    {
        if (_events.ContainsKey(name))
        {
            _events[name] -= method;

            if(_events[name] == null)
            {
                _events.Remove(name);
            }
        }
    }
    public static void Trigger(string name)
    {
        if(_events.ContainsKey(name))
        {
            _events[name]();
        }
    }
}
