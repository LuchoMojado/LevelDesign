using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float _yMaxRotation, _yMinRotation, _xMaxRotation, _xMinRotation, _wallRunTilt;

    [SerializeField] Player _player;

    float _yRotation, _xRotation;

    private void Start()
    {
        _player.movement.OnRotation += Rotation;
        _player.movement.OnWallRunRotation += WallRunRotation;
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

    public void WallRunRotation(float xAxis, float yAxis)
    {
        float xMax = transform.rotation.x + _xMaxRotation;
        float xMin = transform.rotation.x - _xMinRotation;

        _yRotation += yAxis;
        _yRotation = Mathf.Clamp(_yRotation, _yMinRotation, _yMaxRotation);

        _xRotation += xAxis;
        _xRotation = Mathf.Clamp(_xRotation, xMin, xMax);

        transform.rotation = Quaternion.Euler(-_yRotation, _xRotation, 0f);
    }

    public void StartWallRun(bool right, Vector3 normal)
    {
        StartCoroutine(Rotate(normal));

        if (right)
        {
            transform.rotation = Quaternion.Euler(-_yRotation, _xRotation, _wallRunTilt);
        }
        else
        {
            transform.rotation = Quaternion.Euler(-_yRotation, _xRotation, -_wallRunTilt);
        }
    }

    IEnumerator Rotate(Vector3 normal)
    {
        float time = 0;

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            Quaternion.Lerp(transform.rotation, Quaternion.Euler(normal), time * 2);

            yield return null;
        }
    }

    public void CameraUntilt()
    {
        transform.rotation = Quaternion.Euler(-_yRotation, _xRotation, 0);
    }

    public void ChangeCameraY(float amount)
    {
        transform.position += new Vector3(0, amount, 0);
    }
}
