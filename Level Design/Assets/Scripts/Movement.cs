using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public delegate void FloatsDelegate(float a, float b);
    public event FloatsDelegate OnRotation;
    public event FloatsDelegate OnWallRunRotation;

    float _currentSpeed, _normalSpeed, _sprintSpeed, _xRotation, _mouseSensitivity, _jumpStrength, _maxVel;
    Transform _playerTransform;
    Rigidbody _myRB;
    /*public LayerMask whatisWall;
    public float _maxWallrunTime, wallrunForce, currentWallRunTime, maxWallSpeed;
    bool isWallRunning = false;*/
    bool IsSliding = false;
    


    public Movement(Transform transform, Rigidbody rigidbody, float speed, float mouseSensitivity, float jumpStrength)
    {
        _playerTransform = transform;
        _myRB = rigidbody;
        _currentSpeed = speed;
        _normalSpeed = speed;
        _sprintSpeed = speed * 1.5f;
        _maxVel = 10;
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

        SpeedLimit();

        //_myRB.MovePosition(_playerTransform.position + direction * _currentSpeed * Time.fixedDeltaTime);
        _myRB.AddForce(direction * _currentSpeed * 5f, ForceMode.Force);
        //Debug.Log("La velocidad es " + _currentSpeed + " y su velocidad max es " + _maxSpeed);
    }

    public void Rotation(float horizontalMouse, float verticalMouse, bool wallRunning)
    {
        _xRotation += horizontalMouse * _mouseSensitivity * Time.deltaTime;

        if (_xRotation >= 360 || _xRotation <= -360)
        {
            _xRotation -= 360 * Mathf.Sign(_xRotation);
        }

        verticalMouse *= _mouseSensitivity * Time.deltaTime;

        if (!wallRunning)
        {
            _playerTransform.rotation = Quaternion.Euler(0, _xRotation, 0);

            OnRotation(_xRotation, verticalMouse);
        }
        else
            OnWallRunRotation(_xRotation, verticalMouse);
    }
    void SpeedLimit()
    {
        Vector3 horizontalVel = new Vector3(_myRB.velocity.x, 0, _myRB.velocity.z);

        if (horizontalVel.magnitude > _maxVel)
        {
            Vector3 limitedVel = horizontalVel.normalized * _maxVel;
            _myRB.velocity = new Vector3(limitedVel.x, _myRB.velocity.y, limitedVel.z);
        }
    }
    public void Sprint()
    {
        _currentSpeed = _sprintSpeed;
    }

    public void StopSprint()
    {
        _currentSpeed = _normalSpeed;
    }

    public bool Jump(bool grappled)
    {
        if (GroundedCheck())
        {
            _myRB.AddForce(Vector3.up * (_jumpStrength));
            return false;
        }
        else if (grappled && _playerTransform.gameObject.GetComponent<Player>().hookHit.point.y > _playerTransform.position.y)
        {
            _myRB.AddForce(Vector3.up * (_jumpStrength * 1.25f));
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GroundedCheck()
    {
        Ray groundedRay = new Ray(_playerTransform.position, -_playerTransform.up);
        bool grounded = Physics.Raycast(groundedRay, 1.1f, 1 << 6);

        return grounded;
    }

    public void MoveToHook(Vector3 dir, float strength)
    {
        _myRB.AddForce(dir * strength);
    }

    public void Slide()
    {

    }
    public void StopSlide()
    {

    }

    public void WallRunnig()
    {

    }
    public void StartWallRun()
    {

    }
    public void StopWallRun()
    {

    }
    public void CheckWall()
    {

    }
}
