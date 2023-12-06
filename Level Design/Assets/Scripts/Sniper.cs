using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Entity
{
    float _aiming;
    [SerializeField] float _fovRadius, _fovAngle, _killTime;
    LineRenderer _line;
    [SerializeField] LayerMask _obstacleLayer;

    void Start()
    {
        _line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3 playerPos = GameManager.instance.player.transform.position;

        if (InFieldOfView(playerPos))
        {
            print("yea");
            _aiming += Time.deltaTime;
            _line.enabled = true;
            _line.SetPosition(0, transform.position);
            _line.SetPosition(1, playerPos + new Vector3(0, 0.6f, 0));
            if (_aiming >= _killTime)
            {
                GameManager.instance.Respawn();
            }
        }
        else
        {
            print("yean't");
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
        Destroy(this);
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
}
