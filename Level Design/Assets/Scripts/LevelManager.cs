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
    [SerializeField] AudioClip[] music;

    AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        ChangeMusic(0);

        _disablerFactory = new Factory<HookDisabler>(_hookDisabler);
        _disablerPool = new ObjectPool<HookDisabler>(_disablerFactory.GetObject, HookDisabler.TurnOff, HookDisabler.TurnOn, 2);

        SpawnDisabler(_hookDisablerInLevel[0], 70, 1.75f);
        SpawnDisabler(_hookDisablerInLevel[1], 60, 1.75f, 4);
    }

    public HookDisabler SpawnDisabler(Transform transform, float radius = 50, float duration = 2.5f, float firstDelay = 0, float cooldown = 7)
    {
        var disabler = _disablerPool.Get();
        disabler.transform.position = transform.position;
        disabler.transform.rotation = transform.rotation;
        disabler.Initialize(_disablerPool, radius, duration, firstDelay, cooldown);

        return disabler;
    }

    public HookDisabler SpawnDisabler(Vector3 pos, float radius = 50, float duration = 2.5f, float firstDelay = 0, float cooldown = 7)
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
        Instantiate(_blackHole, pos, Quaternion.identity);
    }

    public void ChangeMusic(int index)
    {
        _audioSource.clip = music[index];
        _audioSource.Play();
    }
}
