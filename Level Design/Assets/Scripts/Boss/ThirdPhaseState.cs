using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPhaseState : State
{
    public ThirdPhaseState(Boss b, float transitionTime, float wallInterval, float obstacleInterval, float retreatInterval,
        Dictionary<Vector3, Vector3[]> obsDictionary, float zWallSpawn, float zObstacleSpawn, Transform[] initWallTransform)
    {
        _boss = b;
        _transitionTime = transitionTime;
        _wallSpawnInterval = wallInterval;
        _obstacleSpawnInterval = obstacleInterval;
        _retreatInterval = retreatInterval;
        _obstacleDictionary = obsDictionary;
        _zWallSpawn = zWallSpawn;
        _zObstacleSpawn = zObstacleSpawn;
        _initialWallTransform = initWallTransform;
    }
    
    Dictionary<Vector3, Vector3[]> _obstacleDictionary;
    Transform[] _initialWallTransform;
    float _transitionTime, _obstacleSpawnInterval, _retreatInterval, _wallSpawnInterval, _zWallSpawn, _zObstacleSpawn, _timer;
    bool _transitioning;

    public override void OnEnter()
    {
        _transitioning = true;
        _boss.StartCoroutine(_boss.ThirdPhaseTransition());
        _boss.StartCoroutine(FirstWallBatch());
        _boss.StartCoroutine(RetreatObstacleSpawn());

        _timer = _transitionTime;
    }

    public override void OnUpdate()
    {
        if (!_transitioning)
        {
            if (Vector3.Distance(_boss.transform.position, _boss.playerPos) <= 20)
            {
                _boss.StartCoroutine(_boss.Die());
                _transitioning = true;
            }

            if (_timer <= 0)
            {
                _boss.SpawnObstacle(Random.Range(0, _obstacleDictionary.Count), _obstacleDictionary, _zObstacleSpawn);
                _boss.SpawnWall(_zWallSpawn);

                _timer = _obstacleSpawnInterval;
            }
            else
            {
                _timer -= Time.deltaTime;
            }
        }
        
    }

    public override void OnExit()
    {

    }

    IEnumerator FirstWallBatch()
    {
        yield return new WaitForSeconds(_transitionTime);

        for (int i = 0; i < _initialWallTransform.Length; i++)
        {
            _boss.SpawnWall(_initialWallTransform[i].position.z);
        }
    }

    IEnumerator WallSpawnCycle()
    {
        _boss.SpawnWall(_zWallSpawn);

        yield return new WaitForSeconds(_wallSpawnInterval);

        _boss.StartCoroutine(WallSpawnCycle());
    }

    IEnumerator RetreatObstacleSpawn()
    {
        float timer = 0;
        int length = Mathf.FloorToInt(_transitionTime / _retreatInterval);

        for (int i = 2; i < length; i++)
        {
            float currentGoal = _retreatInterval * (i + 1);

            while (timer < currentGoal)
            {
                timer += Time.deltaTime;

                yield return null;
            }

            _boss.SpawnObstacle(Random.Range(0, _obstacleDictionary.Count), _obstacleDictionary, _boss.transform.position.z - 20, _transitionTime - timer);
        }

        _transitioning = false;
    }
}
