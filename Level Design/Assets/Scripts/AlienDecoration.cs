using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienDecoration : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;

    private int currentWaypointIndex = 0;

    void Update()
    {
        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            // Calcula la dirección hacia el waypoint actual
            Vector3 direction = waypoints[currentWaypointIndex].position - transform.position;

            // Normaliza la dirección y aplica velocidad
            transform.Translate(direction.normalized * speed * Time.deltaTime);

            // Si el alien está lo suficientemente cerca del waypoint, pasa al siguiente
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.2f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            // Reinicia al primer waypoint cuando ha alcanzado el último
            currentWaypointIndex = 0;
        }
    }
}
