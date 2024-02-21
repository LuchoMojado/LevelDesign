using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondPhaseState : State
{
    public SecondPhaseState(Boss b, float transitionT, float obstacleInterval, float retreatInterval, Dictionary<Vector3, Vector3[]> obsDictionary, float zSpawn)
    {
        _boss = b;
        _spawnInterval = obstacleInterval;
        _retreatInterval = retreatInterval;
        _obstacleDictionary = obsDictionary;
        _zSpawnValue = zSpawn;
        _transitionTime = transitionT;
    }

    float _timer, _transitionTime, _spawnInterval, _retreatInterval, _zSpawnValue;
    Dictionary<Vector3, Vector3[]> _obstacleDictionary;
    bool _transitioning;

    public override void OnEnter()
    {
        _timer = _transitionTime * 2 + _spawnInterval;
        _transitioning = true;

        _boss.StartCoroutine(_boss.SecondPhaseTransition());
        _boss.StartCoroutine(RetreatSpawn());
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(_boss.transform.position, _boss.playerPos) <= 40)
        {
            if (!_transitioning)
            {
                fsm.ChangeState(Boss.BossStates.ThirdPhase);
            }
        }

        if (_timer <= 0)
        {
            ObstacleSpawn(_zSpawnValue);

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

    IEnumerator RetreatSpawn()
    {
        yield return new WaitForSeconds(_transitionTime);

        float timer = 0;
        int length = Mathf.FloorToInt(_transitionTime / _retreatInterval);

        for (int i = 0; i < length; i++)
        {
            float currentGoal = _retreatInterval * (i + 1);

            while (timer < currentGoal)
            {
                timer += Time.deltaTime;

                yield return null;
            }

            ObstacleSpawn(_boss.transform.position.z - 20, _transitionTime - timer);
        }

        _transitioning = false;
    }

    void ObstacleSpawn(float zValue)
    {
        int keyNumber = Random.Range(0, _obstacleDictionary.Count);

        _boss.SpawnObstacle(keyNumber, _obstacleDictionary, zValue);

        if (Random.Range(0, 2) == 0)
        {
            int newKeyNumber;

            do
            {
                newKeyNumber = Random.Range(0, _obstacleDictionary.Count);
            } while (newKeyNumber == keyNumber);

            _boss.SpawnObstacle(newKeyNumber, _obstacleDictionary, zValue);
        }
    }

    void ObstacleSpawn(float zValue, float startDelay)
    {
        int keyNumber = Random.Range(0, _obstacleDictionary.Count);

        _boss.SpawnObstacle(keyNumber, _obstacleDictionary, zValue, startDelay);

        if (Random.Range(0, 2) == 0)
        {
            int newKeyNumber;

            do
            {
                newKeyNumber = Random.Range(0, _obstacleDictionary.Count);
            } while (newKeyNumber == keyNumber);

            _boss.SpawnObstacle(newKeyNumber, _obstacleDictionary, zValue, startDelay);
        }
    }
}
