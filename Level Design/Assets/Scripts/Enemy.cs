using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    //[SerializeField] Player _player;

    void Start()
    {
        _hp = FlyweightPointer.Enemy.maxHp;
        EventManager.Subscribe("ILisen", ChasePlayer);
    }

    void Movement(params object[] pos)
    {
        var dir = CheckNearPlayer(pos);
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            transform.forward = dir;
            if (CheckFloor())
            {
                transform.position += transform.forward * FlyweightPointer.Enemy.speed * Time.deltaTime;
            }
        }
    }
    Vector3 CheckNearPlayer(params object[] posPlayer)
    {
        var disToPlayer = Vector3.Distance((Vector3)posPlayer[0], transform.position);
        if (disToPlayer <= FlyweightPointer.Enemy.viewRadius)
        {
            if (disToPlayer <= FlyweightPointer.Enemy.attackRadius)
            {
                Attack();
                return Vector3.zero;
            }
            return ((Vector3)posPlayer[0] - transform.position).normalized;
        }
        return Vector3.zero;
    }

    void ChasePlayer(params object[] posPlayer)
    {
        Movement(posPlayer[0]);
    }
    void Attack()
    {
        Debug.Log("ataco");
    }
    
    bool CheckFloor()
    {
        Ray ray = new Ray(transform.position- new Vector3(0, 0.5f, 0), transform.forward - new Vector3(0, 1.2f, 0));
        bool canMoveForward = Physics.Raycast(ray, 1, 1 << 6);
        return canMoveForward;
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
