using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    float _currentHP;
    //[SerializeField] Player _player;

    void Start()
    {
        _currentHP = FlyweightPointer.Enemy.maxHp;
        //EventManager.Subscribe("ILisen", Movement);
        EventManager.Subscribe("ILisen", ChasePlayer);
    }


    void Update()
    {
        //CheckNearPlayer();
        //Movement();
    }
    void Movement(params object[] pos)
    {
        
        if (CheckFloor())
        {
            var dir = CheckNearPlayer(pos);
            if (dir != Vector3.zero) transform.position += dir * FlyweightPointer.Enemy.speed * Time.deltaTime;
        }
    }
    Vector3 CheckNearPlayer(params object[] posPlayer)
    {
        var disToPlayer = Vector3.Distance((Vector3)posPlayer[0], transform.position);
        if (disToPlayer <= FlyweightPointer.Enemy.viewRadius)
        {
            //Debug.Log("Estoy Cerca");
           return ((Vector3)posPlayer[0] - transform.position).normalized;
        }
        if (disToPlayer <= FlyweightPointer.Enemy.attackRadius)
        {
            //Debug.Log("ataco");
            Attack();
            return Vector3.zero;
        }
        return Vector3.zero;
    }

    void ChasePlayer(params object[] posPlayer)
    {
        Movement(posPlayer[0]);
    }
    void Attack()
    {

    }
    void TakeDamage(float dmg)
    {
        _currentHP -= dmg;
    }
    bool CheckFloor()
    {
        Ray ray = new Ray(transform.position- new Vector3(0, 0.5f, 0), transform.forward - new Vector3(0, 1f, 0));
        bool canMoveForward = Physics.Raycast(ray, 1, 1 << 6);
        return canMoveForward;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position - new Vector3(0, 0.5f, 0), transform.forward);
    }
}
