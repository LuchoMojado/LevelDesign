using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumbersParticleManager : MonoBehaviour
{
    ObjectPool<NumbersParticle> _myPool;
    [SerializeField] NumbersParticle _particle;
    [SerializeField] Factory<NumbersParticle> _factory;

    [SerializeField] float _spawnCooldown;
    float _currentCooldown = 0;

    [SerializeField] float _xMin, _xMax, _y, _zMin, _zMax;

    void Start()
    {
        _factory = new Factory<NumbersParticle>(_particle);
        _myPool = new ObjectPool<NumbersParticle>(_factory.GetObject, NumbersParticle.TurnOff, NumbersParticle.TurnOn, 20);
    }

    private void Update()
    {
        if (_currentCooldown <= 0)
        {
            var x = _myPool.Get();
            x.Initialize(_myPool);
            x.transform.position = GetSpawnPos();
            _currentCooldown = _spawnCooldown;
        }
        else
        {
            _currentCooldown -= Time.deltaTime;
        }
    }

    Vector3 GetSpawnPos()
    {
        Vector3 pos;
        do
        {
            pos = new Vector3(Random.Range(_xMin, _xMax), _y, Random.Range(_zMin, _zMax));
        } while (Physics.Raycast(pos, Vector3.down));

        return pos;
    }
}
