using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    FiniteStateMachine _fsm;

    public List<Renderer> tiles;
    [SerializeField] Material _damagedTileMat;
    [SerializeField] HookDisabler _hookDisabler;
    [SerializeField] float _tileDestroyDelay;
    [SerializeField] Proyectile _proyectile;

    [Header("Hands")]
    [SerializeField] BossHands[] _hands;
    [SerializeField] float _slamPrepareSpeed, _slamSpeed, _sweepPrepareSpeed, _sweepSpeed, _retractSpeed, _slamPrepareTime, _sweepPrepareTime, _recoverTime, _spawnProyectileSpeed, _spawnPrepareTime, _spawnPrepareSpeed;
    [SerializeField] Transform[] _prepareSlamTransform, _idleTransform, _proyectileSpawnTransform;
    [SerializeField] Transform _sweepLimitRight, _sweepLimitLeft, _sweepLimitFront, _sweepLimitBack, _disablerSpawnTransform;

    public float restTime, hookTimeToDisable;

    public Vector3 playerPos;
    public bool takingAction;

    public enum BossStates
    {
        Waiting,
        FirstPhase,
        SecondPhase,
        ThirdPhase
    }

    void Start()
    {
        _fsm = new FiniteStateMachine();

        _fsm.AddState(BossStates.Waiting, new WaitingState(this));
        _fsm.AddState(BossStates.FirstPhase, new FirstPhaseState(this, UseFirstPhaseAction));
        _fsm.AddState(BossStates.SecondPhase, new SecondPhaseState(this));
        _fsm.AddState(BossStates.ThirdPhase, new ThirdPhaseState(this));
        _fsm.ChangeState(BossStates.Waiting);
    }

    void Update()
    {
        playerPos = GameManager.instance.player.transform.position;

        _fsm.Update();
    }

    public IEnumerator FistSlam(int handIndex)
    {
        int index;

        if (_hands[handIndex].busy)
        {
            index = PickFreeHand();
        }
        else
        {
            index = handIndex;
        }

        _hands[index].busy = true;
        takingAction = true;

        StartCoroutine(_hands[index].MoveAndRotate(_prepareSlamTransform[index], _slamPrepareSpeed, true));

        while (_hands[index].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_slamPrepareTime);

        StartCoroutine(_hands[index].MoveAndRotate(new Vector3(playerPos.x, 49, playerPos.z), Quaternion.identity, _slamSpeed));

        while (_hands[index].moving)
        {
            yield return null;
        }

        foreach (var item in tiles)
        {
            if (Vector3.Distance(item.transform.position, _hands[index].transform.position) <= 8)
            {
                StartCoroutine(DestroyTile(item));
            }
        }

        yield return new WaitForSeconds(_recoverTime);

        StartCoroutine(_hands[index].MoveAndRotate(_idleTransform[index], _retractSpeed, true));

        while (_hands[index].moving)
        {
            yield return null;
        }

        _hands[index].busy = false;
        takingAction = false;
    }

    public IEnumerator HandSweep(int handIndex)
    {
        int index;

        if (_hands[handIndex].busy)
        {
            index = PickFreeHand();
        }
        else
        {
            index = handIndex;
        }

        _hands[index].busy = true;
        takingAction = true;
        bool right = index % 2 == 0 ? true : false;
        float xStart, xEnd, zEnd;

        if (right)
        {
            xStart = _sweepLimitRight.position.x;
            xEnd = _sweepLimitLeft.position.x;
        }
        else
        {
            xStart = _sweepLimitLeft.position.x;
            xEnd = _sweepLimitRight.position.x;
        }

        StartCoroutine(_hands[index].MoveAndRotate(new Vector3(xStart, 49, playerPos.z), Quaternion.identity, _sweepPrepareSpeed));

        while (_hands[index].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_sweepPrepareTime);

        zEnd = _hands[index].transform.position.z < playerPos.z ? _sweepLimitBack.position.z : _sweepLimitFront.position.z;

        StartCoroutine(_hands[index].Sweep(playerPos, xEnd, zEnd, _sweepSpeed));

        while (_hands[index].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_recoverTime);

        StartCoroutine(_hands[index].MoveAndRotate(_idleTransform[index], _retractSpeed, true));

        while (_hands[index].moving)
        {
            yield return null;
        }

        _hands[index].busy = false;
        takingAction = false;
    }

    public IEnumerator SpawnProyectiles(int handIndex)
    {
        _hands[handIndex].busy = true;
        takingAction = true;
        bool right = handIndex % 2 == 0 ? true : false;

        Vector3 startPos = right ? _proyectileSpawnTransform[0].position : _proyectileSpawnTransform[_proyectileSpawnTransform.Length - 1].position;
        startPos += Vector3.forward * 5;
        Quaternion rotation = right ? _proyectileSpawnTransform[0].rotation : _proyectileSpawnTransform[_proyectileSpawnTransform.Length - 1].rotation;

        StartCoroutine(_hands[handIndex].MoveAndRotate(startPos, rotation, _spawnPrepareSpeed));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_spawnPrepareTime);

        if (right)
        {
            for (int i = 0; i < _proyectileSpawnTransform.Length; i++)
            {
                Vector3 goal = _proyectileSpawnTransform[i].position + Vector3.forward * 5;

                StartCoroutine(_hands[handIndex].MoveAndRotate(goal, _spawnProyectileSpeed));

                while (_hands[handIndex].moving)
                {
                    yield return null;
                }

                var proyectile = Instantiate(_proyectile, _proyectileSpawnTransform[i]);
                proyectile.boss = this;
            }
        }
        else
        {
            for (int i = _proyectileSpawnTransform.Length - 1; i >= 0; i--)
            {
                Vector3 goal = _proyectileSpawnTransform[i].position + Vector3.forward * 5;

                StartCoroutine(_hands[handIndex].MoveAndRotate(goal, _spawnProyectileSpeed));

                while (_hands[handIndex].moving)
                {
                    yield return null;
                }

                var proyectile = Instantiate(_proyectile, _proyectileSpawnTransform[i]);
                proyectile.boss = this;
            }
        }

        StartCoroutine(_hands[handIndex].MoveAndRotate(_idleTransform[handIndex], _retractSpeed, true));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        _hands[handIndex].busy = false;
        takingAction = false;
    }

    public IEnumerator DisableHook(int handIndex)
    {
        _hands[handIndex].busy = true;
        bool right = handIndex % 2 == 0 ? true : false;

        Vector3 startPos = _disablerSpawnTransform.position - Vector3.up * 4;
        Quaternion rotation = right ? _disablerSpawnTransform.rotation : _disablerSpawnTransform.rotation * new Quaternion(1, 1, -1, 1);

        StartCoroutine(_hands[handIndex].MoveAndRotate(startPos, rotation, _spawnPrepareSpeed));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        var disabler = Instantiate(_hookDisabler, _disablerSpawnTransform.position, Quaternion.identity);

        yield return new WaitForSeconds(4);

        StartCoroutine(_hands[handIndex].MoveAndRotate(disabler.transform.position, _spawnPrepareSpeed));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        disabler.Die();

        StartCoroutine(_hands[handIndex].MoveAndRotate(_idleTransform[handIndex], _retractSpeed, true));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        _hands[handIndex].busy = false;
    }

    public void UseFirstPhaseAction()
    {
        int action = Random.Range(0, 3);

        switch (action)
        {
            case 0:
                StartCoroutine(FistSlam(PickHandBySide()));
                break;
            case 1:
                StartCoroutine(HandSweep(PickHandBySide()));
                break;
            case 2:
                StartCoroutine(SpawnProyectiles(PickFreeHand()));
                break;
            default:
                break;
        }
    }

    public int PickHandBySide()
    {
        int index;

        if (playerPos.x <= transform.position.x)
        {
            do
            {
                index = Random.Range(0, _hands.Length);
            } while (index % 2 != 0);
        }
        else
        {
            do
            {
                index = Random.Range(0, _hands.Length);
            } while (index % 2 == 0);
        }
        
        return index;
    }

    public int PickFreeHand()
    {
        int index;

        do
        {
            index = Random.Range(0, _hands.Length);
        } while (_hands[index].busy == true);

        return index;
    }

    public IEnumerator DestroyTile(Renderer tile)
    {
        tile.material = _damagedTileMat;

        yield return new WaitForSeconds(_tileDestroyDelay);

        tiles.Remove(tile);
        //spawneo particulas
        Destroy(tile.gameObject);
    }
}
