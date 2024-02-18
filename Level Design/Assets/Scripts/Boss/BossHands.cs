using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHands : MonoBehaviour
{
    [HideInInspector] public bool moving { get; private set; }
    [HideInInspector] public bool busy;

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

                transform.rotation = Quaternion.Slerp(transform.rotation, goalTransform.rotation, delta / currentDist);

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

        var dir = new Vector3(playerPos.x - transform.position.x, 0, playerPos.z - transform.position.z);

        while (Mathf.Abs(Mathf.Abs(endPosX) - Mathf.Abs(transform.position.x)) > 0.3f && Mathf.Abs(Mathf.Abs(endPosZ) - Mathf.Abs(transform.position.z)) > 0.3f)
        {
            transform.position += dir * speed * Time.deltaTime;

            yield return null;
        }

        moving = false;
    }
}
