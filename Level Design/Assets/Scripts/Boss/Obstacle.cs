using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    ObjectPool<Obstacle> _objectPool;
    [SerializeField] float _speed;
    public float startDelay;

    IEnumerator Move()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            transform.position += Vector3.back * _speed * Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator ExpandY(float yEndValue)
    {
        float time = 0;
        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);

        while (transform.localScale.y < yEndValue)
        {
            time += Time.deltaTime;

            transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(0, yEndValue, time), transform.localScale.z);

            yield return null;
        }

        StartCoroutine(Move());
    }

    public void Initialize(ObjectPool<Obstacle> op)
    {
        _objectPool = op;

        StartCoroutine(Move());
    }

    public void Initialize(ObjectPool<Obstacle> op, float yScale)
    {
        _objectPool = op;

        StartCoroutine(ExpandY(yScale));
    }

    public static void TurnOff(Obstacle x)
    {
        x.gameObject.SetActive(false);
    }
    public static void TurnOn(Obstacle x)
    {
        x.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            _objectPool.RefillStock(this);
        }
    }
}
