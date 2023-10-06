using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    protected float _hp;

    public virtual void TakeDamage(float dmg)
    {
        _hp -= dmg;
        if (_hp <= 0)
        {
            Die();
        }
    }

    public abstract void Die();
}
