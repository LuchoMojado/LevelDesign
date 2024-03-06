using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MementoState
{
    //VOLVER TIEMPO ATRAS
    List<ParamsMemento> _parameters = new List<ParamsMemento>();
    public void Rec(params object[] parameter)
    {
        if (_parameters.Count >= 1)
            _parameters.RemoveAt(0);
        var remember = new ParamsMemento(parameter);
        _parameters.Add(remember);
    }

    //No guardo nada todavia
    public bool IsRemember()
    {
        return _parameters.Count > 0;
    }

    public ParamsMemento Remember()
    {
        //Cuando vos cargas la partida se borra
        //var x = _parameters;
        //_parameters = null;
        //return x;

        var x = _parameters[_parameters.Count - 1];
        _parameters.RemoveAt(_parameters.Count - 1);
        return x;

        //Normal
        //return _parameters;
    }

    public ParamsMemento Remember(bool forget)
    {
        //Cuando vos cargas la partida se borra
        //var x = _parameters;
        //_parameters = null;
        //return x;

        var x = _parameters[_parameters.Count - 1];

        if (forget)
        {
            _parameters.RemoveAt(_parameters.Count - 1);
        }
        
        return x;

        //Normal
        //return _parameters;
    }
}
