using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHands : Rewind, IPlaySound
{
    [HideInInspector] public bool moving { get; private set; }
    [HideInInspector] public bool busy;
    [SerializeField] GameObject[] _handState;
    AudioSource _audioSource;
    bool _loading;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public enum HandStates
    {
        Closed,
        Open,
        Idle
    }

    public IEnumerator MoveAndRotate(Transform goalTransform, float speed, bool rotate)
    {
        moving = true;

        if (rotate)
        {
            float startDist = Vector3.Distance(transform.position, goalTransform.position);

            while (transform.position != goalTransform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, goalTransform.position, Time.deltaTime * speed);

                float currentDist = Vector3.Distance(transform.position, goalTransform.position);

                float delta = 1 - Mathf.Pow(currentDist / startDist, 5.0f / 9.0f);

                transform.rotation = Quaternion.Slerp(transform.rotation, goalTransform.rotation, (delta / currentDist) * Time.deltaTime * speed);

                yield return null;
            }
        }
        else
        {
            while (transform.position != goalTransform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, goalTransform.position, Time.deltaTime * speed);

                yield return null;
            }
        }

        moving = false;
    }

    public IEnumerator MoveAndRotate(Vector3 goalPosition, Quaternion goalRotation, float speed)
    {
        moving = true;
        float startDist = Vector3.Distance(transform.position, goalPosition);
        
        while (transform.position != goalPosition)
        {
            float currentDist = Vector3.Distance(transform.position, goalPosition);

            float delta = 1 - Mathf.Pow(currentDist / startDist, 5.0f / 9.0f);

            transform.position = Vector3.MoveTowards(transform.position, goalPosition, Time.deltaTime * speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, goalRotation, delta / startDist);

            yield return null;
        }

        moving = false;
    }

    public IEnumerator MoveAndRotate(Vector3 goalPosition, float speed)
    {
        moving = true;

        while (transform.position != goalPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, goalPosition, Time.deltaTime * speed);

            yield return null;
        }

        moving = false;
    }

    public IEnumerator Sweep(Vector3 playerPos, float endPosX, float endPosZ, float speed)
    {
        moving = true;

        var dir = new Vector3(playerPos.x - transform.position.x, 0, (playerPos.z + 3) - transform.position.z).normalized;

        while (Mathf.Abs(Mathf.Abs(endPosX) - Mathf.Abs(transform.position.x)) > 0.3f && Mathf.Abs(Mathf.Abs(endPosZ) - Mathf.Abs(transform.position.z)) > 0.3f)
        {
            transform.position += dir * speed * Time.deltaTime;

            yield return null;
        }

        moving = false;
    }

    public void ChangeHandState(HandStates newState)
    {
        switch (newState)
        {
            case HandStates.Closed:
                _handState[0].SetActive(true);
                _handState[1].SetActive(false);
                _handState[2].SetActive(false);
                break;
            case HandStates.Open:
                _handState[0].SetActive(false);
                _handState[1].SetActive(true);
                _handState[2].SetActive(false);
                break;
            case HandStates.Idle:
                _handState[0].SetActive(false);
                _handState[1].SetActive(false);
                _handState[2].SetActive(true);
                break;
            default:
                break;
        }
    }
    
    public override void Save()
    {
        if (_loading)
            return;
        _mementoState.Rec(transform.position,transform.rotation);
    }

    public override void Load()
    {
        StopAllCoroutines();
        moving = false;
        busy = false;
        if (_mementoState.IsRemember())
        {
            StartCoroutine(CoroutineLoad());
        }
    }

    IEnumerator CoroutineLoad()
    {
        var WaitForSeconds = new WaitForSeconds(0.01f);
        _loading = true;
        //_myRB.constraints = RigidbodyConstraints.FreezeAll;
        while (_mementoState.IsRemember())
        {
            var data = _mementoState.Remember();
            //_loading = true;
            //Pongo en el array la pos que se donde lo puse lo que quiero
            transform.position = (Vector3)data.parameters[0];
            transform.rotation = (Quaternion)data.parameters[1];

            yield return WaitForSeconds;
        }
        //_myRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _loading = false;
    }

    public void PlaySound(AudioClip clip, bool loop)
    {
        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.Play();
    }

    public void StopSound()
    {
        _audioSource.Stop();
    }
}
