using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float _yMaxRotation, _yMinRotation, _xMaxRotation, _xMinRotation, _wallRunTilt;
    float _tilt, _xMax, _xMin;

    [SerializeField] Player _player;

    float _yRotation, _xRotation;

    float _yChange;

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
        if (_yChange != 0)
        {
            transform.position = _player.transform.position + new Vector3(0, _yChange, 0);
        }
    }

    public void Rotation(float xAxis, float yAxis)
    {
        _yRotation += yAxis;

        _yRotation = Mathf.Clamp(_yRotation, _yMinRotation, _yMaxRotation);

        transform.rotation = Quaternion.Euler(-_yRotation, xAxis, 0f);
    }

    public void WallRunRotation(float xAxis, float yAxis)
    {
        _yRotation += yAxis;
        _yRotation = Mathf.Clamp(_yRotation, _yMinRotation, _yMaxRotation);

        _xRotation += xAxis * 0.25f;
        _xRotation = Mathf.Clamp(_xRotation, _xMin, _xMax);

        transform.rotation = Quaternion.Euler(-_yRotation, _xRotation, _tilt);
    }

    public void StartWall(bool right, float angle)
    {
        StartCoroutine(Rotate(angle));

        if (right)
        {
            _tilt = _wallRunTilt;
        }
        else
        {
            _tilt = -_wallRunTilt;
        }
    }

    IEnumerator Rotate(float angle)
    {
        float time = 0;
        var oldRot = transform.rotation;

        while (time < 0.1f)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(oldRot, _player.transform.rotation, time * 10);

            _xMin = transform.eulerAngles.y + _xMinRotation;
            _xMax = transform.eulerAngles.y + _xMaxRotation;

            yield return null;
        }
    }

    public void CameraUntilt()
    {
        transform.rotation = Quaternion.Euler(-_yRotation, _xRotation, 0);
    }

    public void ChangeCameraY(float amount)
    {
        _yChange = amount;
    }
}
