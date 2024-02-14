using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumbersParticle : MonoBehaviour
{
    [SerializeField] float _speed, _lifetime;
    ObjectPool<NumbersParticle> _objectPool;
    ParticleSystem _particles;
    float _timer;

    void Update()
    {
        transform.position -= transform.up * _speed * Time.deltaTime;
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _particles.Stop();

            if (_timer <= -6.5f)
            {
                _particles.Play();
                _timer = _lifetime;
                _objectPool.RefillStock(this);
            }
        }
    }

    public void Initialize(ObjectPool<NumbersParticle> op)
    {
        _particles = GetComponent<ParticleSystem>();
        _objectPool = op;
        _timer = _lifetime;
    }

    public static void TurnOff(NumbersParticle x)
    {
        x.gameObject.SetActive(false);
    }
    public static void TurnOn(NumbersParticle x)
    {
        
        x.gameObject.SetActive(true);
    }
}
