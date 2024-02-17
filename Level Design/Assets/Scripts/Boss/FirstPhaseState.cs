using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FirstPhaseState : State
{
    public FirstPhaseState(Boss b, Action firstPhaseActions)
    {
        _boss = b;
        _takeAction = firstPhaseActions;
    }

    float _timer;
    Action _takeAction;

    public override void OnEnter()
    {
        _timer = _boss.restTime;
    }

    public override void OnUpdate()
    {
        _boss.playerPos = GameManager.instance.player.transform.position;

        _boss.transform.forward = _boss.playerPos - _boss.transform.position;

        if (_boss.takingAction)
        {
            return;
        }

        if (_timer <= 0)
        {
            _takeAction();
            _timer = _boss.restTime;
        }
        else
        {
            _timer -= Time.deltaTime;
        }

        if (_boss.tiles.Count <= 0)
        {
            fsm.ChangeState(Boss.BossStates.SecondPhase);
        }
    }

    public override void OnExit()
    {

    }
}
