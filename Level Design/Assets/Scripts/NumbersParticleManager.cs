using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumbersParticleManager : MonoBehaviour
{
    ObjectPool<NumbersParticle> myPool;
    [SerializeField] NumbersParticle particle;
    [SerializeField] Factory<NumbersParticle> factory;

    [SerializeField] float _spawnCooldown;
    float _currentCooldown = 0;

    [SerializeField] float _xMin, _xMax, _y, _zMin, _zMax;

    void Start()
    {
        factory = new Factory<NumbersParticle>(particle);
        myPool = new ObjectPool<NumbersParticle>(factory.GetObject, NumbersParticle.TurnOff, NumbersParticle.TurnOn, 20);
    }

    private void Update()
    {
        if (_currentCooldown <= 0)
        {
            var x = myPool.Get();
            x.Initialize(myPool);
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
