using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    float _currentHP;
    [SerializeField] Player _player;

    void Start()
    {
        _currentHP = FlyweightPointer.Enemy.maxHp;
    }


    void Update()
    {
        CheckNearPlayer();
        Movement();
    }
    void Movement()
    {
        
        if (CheckFloor())
        {
            var dir = CheckNearPlayer();
            if (dir != Vector3.zero) transform.position += dir;
        }
    }
    Vector3 CheckNearPlayer()
    {
        var disToPlayer = Vector3.Distance(_player.transform.position, transform.position);
        if (disToPlayer <= FlyweightPointer.Enemy.viewRadius)
        {
            //Debug.Log("Estoy Cerca");
           return (_player.transform.position-transform.position).normalized* FlyweightPointer.Enemy.speed*Time.deltaTime;
        }
        if (disToPlayer <= FlyweightPointer.Enemy.attackRadius)
        {
            //Debug.Log("ataco");
            Attack();
            return Vector3.zero;
        }
        return Vector3.zero;
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
