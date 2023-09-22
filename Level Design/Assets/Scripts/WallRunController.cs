using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallRunController : MonoBehaviour
{
    public float wallRunDuration = 10f; // Duraci�n m�xima del wall run en segundos
    public float fallSpeed = 1f; // Velocidad de ca�da despu�s del wall run

    private bool isWallRunning = false;
    private float wallRunStartTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            StartWallRun();
        }
    }

    private void Update()
    {
        if (isWallRunning)
        {
            // Verificar si ha pasado el tiempo m�ximo del wall run
            if (Time.time - wallRunStartTime >= wallRunDuration)
            {
                StopWallRun();
            }
            else
            {
                // Aplicar movimiento lateral mientras hace wall run
                // Aqu� puedes implementar el movimiento lateral a lo largo de la pared.
            }
        }
        else
        {
            // Aplicar gravedad cuando no est� haciendo wall run
            Vector3 gravity = new Vector3(0, -fallSpeed, 0);
            GetComponent<CharacterController>().Move(gravity * Time.deltaTime);
        }
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        wallRunStartTime = Time.time;
    }

    private void StopWallRun()
    {
        isWallRunning = false;
    }
}

