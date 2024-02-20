using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SecondPhaseState : State
{
    public SecondPhaseState(Boss b, float transitionT, float obstacleInterval, Dictionary<Vector3, Vector3[]> obsDictionary, Obstacle obs, float zSpawn)
    {
        _boss = b;
        _spawnInterval = obstacleInterval;
        _obstacleDictionary = obsDictionary;
        _obstacle = obs;
        _zSpawnValue = zSpawn;
        _transitionTime = transitionT;
    }

    float _timer, _transitionTime, _spawnInterval, _zSpawnValue;
    Dictionary<Vector3, Vector3[]> _obstacleDictionary;
    Obstacle _obstacle;
    ObjectPool<Obstacle> _obstaclePool;
    Factory<Obstacle> _obstacleFactory;

    public override void OnEnter()
    {
        _obstacleFactory = new Factory<Obstacle>(_obstacle);
        _obstaclePool = new ObjectPool<Obstacle>(_obstacleFactory.GetObject, Obstacle.TurnOff, Obstacle.TurnOn, 14);

        _boss.StartCoroutine(_boss.SecondPhaseTransition());

        _timer = _transitionTime;
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(_boss.transform.position, _boss.playerPos) <= 40)
        {
            fsm.ChangeState(Boss.BossStates.ThirdPhase);
        }

        if (_timer <= 0)
        {
            int keyNumber = Random.Range(0, _obstacleDictionary.Count);

            SpawnObstacle(keyNumber);

            if (Random.Range(0,2) == 0)
            {
                int newKeyNumber;

                do
                {
                    newKeyNumber = Random.Range(0, _obstacleDictionary.Count);
                } while (newKeyNumber == keyNumber);

                SpawnObstacle(newKeyNumber);
            }

            _timer = _spawnInterval;
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }

    public override void OnExit()
    {
        Debug.Log("third phase");
    }

    void SpawnObstacle(int keyNumber)
    {
        Vector3 scale = _obstacleDictionary.ElementAt(keyNumber).Key;
        Vector3 pos = _obstacleDictionary[scale][Random.Range(0, _obstacleDictionary[scale].Length)];
        pos += Vector3.forward * _zSpawnValue;

        var obs = _obstaclePool.Get();
        obs.transform.localScale = scale;
        obs.transform.position = pos;
        obs.Initialize(_obstaclePool);
    } 
}
