using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour
{
    [SerializeField] float _speed, _shootDelay, _despawnTime;
    [HideInInspector] public Boss boss;

    void Start()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        float timer = _despawnTime;

        yield return new WaitForSeconds(_shootDelay);

        var dir = (GameManager.instance.player.transform.position - transform.position).normalized;

        while (timer > 0)
        {
            transform.position += dir * _speed * Time.deltaTime;

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (other.TryGetComponent(out Player player))
        {
            player.TakeDamage(1);
            Destroy(gameObject);
            return;
        }*/

        foreach (var item in boss.tiles)
        {
            if (Vector3.Distance(item.transform.position, transform.position) <= 3.25f)
            {
                boss.StartCoroutine(boss.DestroyTile(item));
                Destroy(gameObject);
                return;
            }
        }
    }
}
