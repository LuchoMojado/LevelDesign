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

    float _timer, _hookedTime;
    Action _takeAction;
    Player _player;

    public override void OnEnter()
    {
        _timer = _boss.restTime;

        _player = GameManager.instance.player;
    }

    public override void OnUpdate()
    {
        _boss.playerPos = _player.transform.position;

        _boss.transform.forward = _boss.playerPos - _boss.transform.position;

        if (_player._grapplingHook.grappled)
        {
            _hookedTime += Time.deltaTime;

            if(_hookedTime >= _boss.hookTimeToDisable)
            {
                _boss.StartCoroutine(_boss.DisableHook(_boss.PickFreeHand()));

                _hookedTime = 0;
            }
        }

        if (_boss.takingAction)
        {
            return;
        }

        if (_timer <= 0)
        {
            if (_boss.tiles.Count <= 20)
            {
                // destroys remaining tiles (into state change?)
                
                fsm.ChangeState(Boss.BossStates.SecondPhase);
            }

            _takeAction();
            _timer = _boss.restTime;
        }
        else
        {
            _timer -= Time.deltaTime;
        }

        
    }

    public override void OnExit()
    {

    }
}
