using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnergy : MonoBehaviour
{
    [HideInInspector] public Vector3 currentGoal, finalGoal;
    [HideInInspector] public float speed = 1;

    void Update()
    {
        if (currentGoal == null)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, currentGoal, speed * Time.deltaTime);

        if (finalGoal == null)
        {
            return;
        }

        if (Vector3.Distance(transform.position, finalGoal) <= 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
