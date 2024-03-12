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
    [SerializeField] AudioClip[] music;

    AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        ChangeMusic(0);

        _disablerFactory = new Factory<HookDisabler>(_hookDisabler);
        _disablerPool = new ObjectPool<HookDisabler>(_disablerFactory.GetObject, HookDisabler.TurnOff, HookDisabler.TurnOn, 2);

        SpawnDisabler(_hookDisablerInLevel[0], FlyweightPointer.HookDisablerLongRange._disableRadius, FlyweightPointer.HookDisabler._disableDuration);
        SpawnDisabler(_hookDisablerInLevel[1], FlyweightPointer.HookDisabler._disableRadius, FlyweightPointer.HookDisabler._disableDuration, FlyweightPointer.HookDisabler._firstDisableDelay);
    }

    public HookDisabler SpawnDisabler(Transform transform, float radius = 50, float duration = 2.5f, float firstDelay = 0, float cooldown = 6)
    {
        var disabler = _disablerPool.Get();
        disabler.transform.position = transform.position;
        disabler.transform.rotation = transform.rotation;
        disabler.Initialize(_disablerPool, radius, duration, firstDelay, cooldown);

        return disabler;
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
        ChangeMusic(2);

        foreach (var item in _part1Objects)
        {
            item.SetActive(false);
        }

        foreach (var item in _part2Objects)
        {
            item.SetActive(true);
        }


        GameManager.instance.playerDieDistance = -250;
    }

    public void ChangeMusic(int index)
    {
        _audioSource.clip = music[index];
        _audioSource.Play();
    }
}
