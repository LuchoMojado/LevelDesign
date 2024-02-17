using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHands : MonoBehaviour
{
    [HideInInspector] public bool moving { get; private set; }
    [HideInInspector] public bool busy;

    //usar lookup table para el calculo de distancia
    public IEnumerator MoveAndRotate(Transform goalTransform, float speed)
    {
        moving = true;

        while (transform.position != goalTransform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, goalTransform.position, Time.deltaTime * speed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, goalTransform.rotation, Time.deltaTime * speed);

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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, Time.deltaTime * speed);

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
