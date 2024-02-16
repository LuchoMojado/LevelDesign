using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHands : MonoBehaviour
{
    public bool moving { get; private set; }
    
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
}
