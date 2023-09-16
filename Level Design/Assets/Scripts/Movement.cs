using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public delegate void FloatsDelegate(float a, float b);
    public event FloatsDelegate OnRotation;

    float _currentSpeed, _normalSpeed, _sprintSpeed, _xRotation, _mouseSensitivity, _jumpStrength;
    bool _isGrounded;
    Transform _playerTransform;
    Rigidbody _myRB;

    public Movement(Transform transform, Rigidbody rigidbody, float speed, float mouseSensitivity, float jumpStrength)
    {
        _playerTransform = transform;
        _myRB = rigidbody;
        _currentSpeed = speed;
        _normalSpeed = speed;
        _sprintSpeed = speed * 1.5f;
        _mouseSensitivity = mouseSensitivity;
        _jumpStrength = jumpStrength;
    }

    

    public void Move(float horizontalInput, float verticalInput)
    {
        Vector3 direction = _playerTransform.forward * verticalInput + _playerTransform.right * horizontalInput;

        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        _myRB.MovePosition(_playerTransform.position + direction * _currentSpeed * Time.fixedDeltaTime);
    }

    public void Rotation(float horizontalMouse, float verticalMouse)
    {
        _xRotation += horizontalMouse * _mouseSensitivity * Time.deltaTime;

        if (_xRotation >= 360 || _xRotation <= -360)
        {
            _xRotation -= 360 * Mathf.Sign(_xRotation);
        }

        _playerTransform.rotation = Quaternion.Euler(0, _xRotation, 0);

        verticalMouse *= _mouseSensitivity * Time.deltaTime;

        OnRotation(_xRotation, verticalMouse);
    }

    public void Sprint()
    {
        _currentSpeed = _sprintSpeed;
    }

    public void StopSprint()
    {
        _currentSpeed = _normalSpeed;
    }

    public void Jump()
    {
        Ray groundedRay = new Ray(_playerTransform.position, -_playerTransform.up);
        _isGrounded = Physics.Raycast(groundedRay, 1.1f, 512);

        if (_isGrounded)
        {
            _myRB.AddForce(Vector3.up * (_jumpStrength));
        }
    }
}
