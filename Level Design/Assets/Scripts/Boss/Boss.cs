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
    [SerializeField] float _slamPrepareSpeed, _slamSpeed, _sweepPrepareSpeed, _sweepSpeed, _retractSpeed, _slamPrepareTime, _sweepPrepareTime, _recoverTime, _spawnProyectileSpeed;
    [SerializeField] Transform[] _prepareSlamTransform, _idleTransform, _proyectileSpawnTransform;
    [SerializeField] Transform _sweepLimitRight, _sweepLimitLeft, _sweepLimitFront, _sweepLimitBack;

    public float restTime;

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
        takingAction = true;

        StartCoroutine(_hands[handIndex].MoveAndRotate(_prepareSlamTransform[handIndex], _slamPrepareSpeed, true));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_slamPrepareTime);

        StartCoroutine(_hands[handIndex].MoveAndRotate(new Vector3(playerPos.x, 49, playerPos.z), _slamSpeed));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        foreach (var item in tiles)
        {
            if (Vector3.Distance(item.transform.position, _hands[handIndex].transform.position) <= 8)
            {
                StartCoroutine(DestroyTile(item));
            }
        }

        yield return new WaitForSeconds(_recoverTime);

        StartCoroutine(_hands[handIndex].MoveAndRotate(_idleTransform[handIndex], _retractSpeed, true));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        takingAction = false;
    }

    public IEnumerator HandSweep(int handIndex)
    {
        takingAction = true;
        bool right = handIndex % 2 == 0 ? true : false;
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

        StartCoroutine(_hands[handIndex].MoveAndRotate(new Vector3(xStart, 49, playerPos.z), _sweepPrepareSpeed));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_sweepPrepareTime);

        zEnd = _hands[handIndex].transform.position.z < playerPos.z ? _sweepLimitBack.position.z : _sweepLimitFront.position.z;

        StartCoroutine(_hands[handIndex].Sweep(playerPos, xEnd, zEnd, _sweepSpeed));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_recoverTime);

        StartCoroutine(_hands[handIndex].MoveAndRotate(_idleTransform[handIndex], _retractSpeed, true));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        takingAction = false;
    }

    public IEnumerator SpawnProyectiles(int handIndex)
    {
        takingAction = true;
        bool right = handIndex % 2 == 0 ? true : false;

        Vector3 startPos = right ? _proyectileSpawnTransform[0].position : _proyectileSpawnTransform[_proyectileSpawnTransform.Length - 1].position;
        startPos += Vector3.forward * 5;
        Quaternion rotation = new Quaternion(90, 0, 90, 0);

        StartCoroutine(_hands[handIndex].MoveAndRotate(startPos, rotation, _spawnProyectileSpeed));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);

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

        StartCoroutine(_hands[handIndex].MoveAndRotate(_idleTransform[handIndex], _spawnProyectileSpeed, true));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        takingAction = false;
    }

    public void UseFirstPhaseAction()
    {
        int action = Random.Range(0, 2);

        switch (action)
        {
            case 0:
                StartCoroutine(FistSlam(PickHandBySide()));
                break;
            case 1:
                StartCoroutine(HandSweep(PickHandBySide()));
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

    public IEnumerator DestroyTile(Renderer tile)
    {
        tile.material = _damagedTileMat;

        yield return new WaitForSeconds(_tileDestroyDelay);

        tiles.Remove(tile);
        //spawneo particulas
        Destroy(tile.gameObject);
    }
}
