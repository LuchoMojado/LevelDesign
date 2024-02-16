using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPhaseState : State
{
    public FirstPhaseState(Boss b)
    {
        _boss = b;
    }

    float _timer;

    public override void OnEnter()
    {
        Debug.Log("first phase");
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
            _boss.StartCoroutine(_boss.FistSlam(_boss.PickHand()));
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
