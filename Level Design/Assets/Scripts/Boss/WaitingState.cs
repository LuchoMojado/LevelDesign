using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingState : State
{
    public WaitingState(Boss b)
    {
        _boss = b;
    }

    public override void OnEnter()
    {
        Debug.Log("waiting");
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(_boss.transform.position, _boss.playerPos) <= 72)
        {
            Debug.Log("player detectado");
            fsm.ChangeState(Boss.BossStates.FirstPhase);
        }
    }

    public override void OnExit()
    {
        
    }
}
