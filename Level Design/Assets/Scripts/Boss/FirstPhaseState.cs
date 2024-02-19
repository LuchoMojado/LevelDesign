using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FirstPhaseState : State
{
    public FirstPhaseState(Boss b, Action firstPhaseActions, float maxHookTime, int minTiles, float restDuration)
    {
        _boss = b;
        _takeAction = firstPhaseActions;
        _hookThreshold = maxHookTime;
        _tilesThreshold = minTiles;
        _actionCD = restDuration;
    }

    float _timer, _hookedTime, _hookThreshold, _actionCD;
    int _tilesThreshold;
    Action _takeAction;
    Player _player;

    public override void OnEnter()
    {
        _timer = _actionCD;

        _player = GameManager.instance.player;
    }

    public override void OnUpdate()
    {
        _boss.playerPos = _player.transform.position;

        _boss.transform.forward = _boss.playerPos - _boss.transform.position;

        if (_player._grapplingHook.grappled)
        {
            _hookedTime += Time.deltaTime;

            if(_hookedTime >= _hookThreshold)
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
            if (_boss.tiles.Count <= _tilesThreshold)
            {
                _boss.ExplodeRemainingTiles();

                fsm.ChangeState(Boss.BossStates.SecondPhase);
            }
            else
            {
                _takeAction();
                _timer = _actionCD;
            }
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
