using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public float maxHp;
    protected float _hp;

    public virtual void TakeDamage(float dmg)
    {
        _hp -= dmg;
    }

    public abstract void Die();
}
