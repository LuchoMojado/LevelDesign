using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondPhaseState : State
{
    public SecondPhaseState(Boss b)
    {
        _boss = b;
    }

    public override void OnEnter()
    {
        Debug.Log("second phase");
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }
}
