using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Entity
{
    float _aiming;
    [SerializeField] float _fovRadius, _fovAngle, _killTime;
    LineRenderer _line;
    [SerializeField] LayerMask _obstacleLayer;
    AudioSource _AS;
    ObjectPool<Sniper> _objectPool;

    public void Initialize(ObjectPool<Sniper> pool)
    {
        _objectPool = pool;
        _AS = GetComponent<AudioSource>();
        _line = GetComponent<LineRenderer>();
        _line.SetPosition(0, transform.position);
    }

    void Update()
    {
        Player player = GameManager.instance.player;
        Vector3 playerPos = player.transform.position;

        if (InFieldOfView(playerPos))
        {
            _aiming += Time.deltaTime;
            _line.enabled = true;
            _line.SetPosition(1, player.transform.position);
            if (_aiming >= _killTime)
            {
                GameManager.instance.Respawn();
                _AS.Play();
            }
        }
        else
        {
            _line.enabled = false;
            _aiming = 0;
        }
    }

    public bool InFieldOfView(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        if (!InLineOfSight(target)) return false;
        if (dir.magnitude > _fovRadius) return false;
        return Vector3.Angle(transform.forward, dir) <= _fovAngle * 0.5f;
    }

    public bool InLineOfSight(Vector3 target)
    {
        var dir = target - transform.position;

        return !Physics.Raycast(transform.position, dir, dir.magnitude, _obstacleLayer);
    }

    public override void Die()
    {
        _objectPool.RefillStock(this);
    }

    public override void Load()
    {
        throw new System.NotImplementedException();
    }

    public override void Save()
    {
        throw new System.NotImplementedException();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            Die();
        }
    }

    public static void TurnOff(Sniper x)
    {
        x.gameObject.SetActive(false);
    }
    public static void TurnOn(Sniper x)
    {
        x.gameObject.SetActive(true);
    }
}
