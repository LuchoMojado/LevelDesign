using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    ObjectPool<HookDisabler> _disablerPool;
    Factory<HookDisabler> _disablerFactory;
    [SerializeField] HookDisabler _hookDisabler;
    [SerializeField] Transform[] _hookDisablerInLevel;
    [SerializeField] GameObject[] _part1Objects, _part2Objects;
    [SerializeField] BlackHole _blackHole;

    void Start()
    {
        _disablerFactory = new Factory<HookDisabler>(_hookDisabler);
        _disablerPool = new ObjectPool<HookDisabler>(_disablerFactory.GetObject, HookDisabler.TurnOff, HookDisabler.TurnOn, 2);

        SpawnDisabler(_hookDisablerInLevel[0].position, 70);
        SpawnDisabler(_hookDisablerInLevel[1].position, 60, 3, 4);
    }

    public HookDisabler SpawnDisabler(Vector3 pos, float radius = 50, float duration = 2.5f, float firstDelay = 0, float cooldown = 6)
    {
        var disabler = _disablerPool.Get();
        disabler.transform.position = pos;
        disabler.Initialize(_disablerPool, radius, duration, firstDelay, cooldown);

        return disabler;
    }

    public void BeginPart2(Vector3 pos)
    {
        foreach (var item in _part1Objects)
        {
            item.SetActive(false);
        }

        foreach (var item in _part2Objects)
        {
            item.SetActive(true);
        }

        Instantiate(_blackHole, pos, Quaternion.identity);
    }
}
