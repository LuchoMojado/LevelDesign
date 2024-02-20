using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    ObjectPool<Obstacle> _objectPool;
    [SerializeField] float _speed, _startDelay, _despawnTime;

    IEnumerator Move()
    {
        float timer = _despawnTime;

        yield return new WaitForSeconds(_startDelay);

        while (timer >= 0)
        {
            timer -= Time.deltaTime;

            transform.position += Vector3.back * _speed * Time.deltaTime;

            yield return null;
        }

        _objectPool.RefillStock(this);
    }

    public void Initialize(ObjectPool<Obstacle> op)
    {
        _objectPool = op;

        StartCoroutine(Move());
    }

    public static void TurnOff(Obstacle x)
    {
        x.gameObject.SetActive(false);
    }
    public static void TurnOn(Obstacle x)
    {
        x.gameObject.SetActive(true);
    }
}
