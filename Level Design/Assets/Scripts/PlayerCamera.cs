using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float _yMaxRotation, _yMinRotation;

    [SerializeField] Player _player;

    float _yRotation;

    private void Start()
    {
        _player.movement.OnRotation += Rotation;
    }

    private void LateUpdate()
    {
        Movement();
    }

    void Movement()
    {
        transform.position = _player.transform.position;
    }

    public void Rotation(float xAxis, float yAxis)
    {
        _yRotation += yAxis;

        _yRotation = Mathf.Clamp(_yRotation, _yMinRotation, _yMaxRotation);

        transform.rotation = Quaternion.Euler(-_yRotation, xAxis, 0f);
    }
}
