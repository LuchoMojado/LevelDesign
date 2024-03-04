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
        _boss.transform.forward = -Vector3.forward;
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(_boss.transform.position, _boss.playerPos) <= 70)
        {
            _boss.StartCoroutine(_boss.FirstPhaseTransition());

            fsm.ChangeState(Boss.BossStates.FirstPhase);
        }
    }

    public override void OnExit()
    {
        
    }
}
