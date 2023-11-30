using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MementoState
{
    public object[] data;
    public void Rec(params object[] parameter)
    {
        data = parameter;
    }

    //No guardo nada todavia
    public bool IsRemember()
    {
        return data.Length > 0;
    }
}
