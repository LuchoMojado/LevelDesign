using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondPhaseState : State
{
    public SecondPhaseState(Boss b, float obstacleInterval)
    {
        _boss = b;
        _spawnInterval = obstacleInterval;
    }

    float _timer, _spawnInterval;

    public override void OnEnter()
    {
        _boss.StartCoroutine(_boss.SecondPhaseTransition());

        _timer = 5;
    }

    public override void OnUpdate()
    {
        if (_timer <= 0)
        {
            // spawn obstacle

            _timer = _spawnInterval;
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
