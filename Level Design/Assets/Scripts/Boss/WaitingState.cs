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
        
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(_boss.transform.position, GameManager.instance.player.transform.position) <= 72)
        {
            fsm.ChangeState(Boss.BossStates.FirstPhase);
        }
    }

    public override void OnExit()
    {
        
    }
}
