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

    [Header("Hands")]
    [SerializeField] BossHands[] _hands;
    [SerializeField] float _prepareSpeed, _slamSpeed, _sweepSpeed, _retractSpeed, _prepareTime, _recoverTime;
    [SerializeField] Transform[] _prepareSlamTransform, _idleTransform;
    [SerializeField] Vector3 _sweepStartPos, _sweepEndPos;

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
        _fsm.AddState(BossStates.FirstPhase, new FirstPhaseState(this));
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

        StartCoroutine(_hands[handIndex].MoveAndRotate(_prepareSlamTransform[handIndex], _prepareSpeed));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_prepareTime);

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

        StartCoroutine(_hands[handIndex].MoveAndRotate(_idleTransform[handIndex], _retractSpeed));

        while (_hands[handIndex].moving)
        {
            yield return null;
        }

        takingAction = false;
    }

    void HandSweep(int handIndex)
    {

    }

    public int PickHand()
    {
        if (playerPos.x <= transform.position.x)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    IEnumerator DestroyTile(Renderer tile)
    {
        tile.material = _damagedTileMat;

        yield return new WaitForSeconds(_tileDestroyDelay);

        tiles.Remove(tile);
        //spawneo particulas
        Destroy(tile.gameObject);
    }
}
