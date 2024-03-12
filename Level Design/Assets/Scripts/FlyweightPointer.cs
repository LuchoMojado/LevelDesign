using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlyweightPointer 
{
    public static readonly Flyweight Enemy = new Flyweight
    {
        speed = 2,
        maxHp = 100,
        viewRadius = 8,
        attackRadius = 1.5f,
        damage = 10,
        enemyColor = Color.cyan
    };
    public static readonly Flyweight EnemyHeavy = new Flyweight
    {
        speed = 1,
        maxHp = 150,
        viewRadius = 8,
        attackRadius = 2,
        damage = 30,
        enemyColor = Color.red
    };
    public static readonly Flyweight EnemyMedium = new Flyweight
    {
        speed = 2.5f,
        maxHp = 125,
        viewRadius = 8,
        attackRadius = 1,
        damage = 15,
        enemyColor = Color.blue
    };
    public static readonly Flyweight EnemyBoss = new Flyweight
    {
        speed = 3,
        maxHp = 150,
        viewRadius = 12,
        attackRadius = 1.5f,
        damage = 20,
        enemyColor = Color.cyan
    };
    public static readonly Flyweight HookDisabler = new Flyweight
    {
        _firstDisableDelay=4,
        _disableDuration=1.75f,
        _disableCooldown=6,
        _disableRadius = 60
    };
    public static readonly Flyweight HookDisablerLongRange = new Flyweight
    {
        _disableRadius = 70
    };
}
