using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour
{
    ObjectPool<Proyectile> _objectPool;
    [SerializeField] float _speed, _shootDelay, _despawnTime;
    Boss _boss;

    IEnumerator Shoot()
    {
        float timer = _despawnTime;

        yield return new WaitForSeconds(_shootDelay);

        var dir = (GameManager.instance.player.transform.position - transform.position).normalized;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            transform.position += dir * _speed * Time.deltaTime;

            yield return null;
        }

        _objectPool.RefillStock(this);
    }

    public void Initialize(ObjectPool<Proyectile> op, Boss boss)
    {
        _objectPool = op;
        _boss = boss;
        StartCoroutine(Shoot());
    }

    public static void TurnOff(Proyectile x)
    {
        x.gameObject.SetActive(false);
    }
    public static void TurnOn(Proyectile x)
    {
        x.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var item in _boss.tiles)
        {
            if (Vector3.Distance(item.transform.position, transform.position) <= 3.25f)
            {
                _boss.StartCoroutine(_boss.DestroyTile(item));
            }
        }

        _objectPool.RefillStock(this);
    }
}
