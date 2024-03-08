using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperSpawner : MonoBehaviour
{
    [SerializeField] Sniper _sniper;
    ObjectPool<Sniper> _sniperPool;
    Factory<Sniper> _sniperFactory;
    [SerializeField] Transform _sniperPos;
    void Start()
    {
        _sniperFactory = new Factory<Sniper>(_sniper);
        _sniperPool = new ObjectPool<Sniper>(_sniperFactory.GetObject, Sniper.TurnOff, Sniper.TurnOn, 1);

        var sniper = _sniperPool.Get();
        sniper.Initialize(_sniperPool);
        sniper.transform.position = _sniperPos.position;
    }
}
