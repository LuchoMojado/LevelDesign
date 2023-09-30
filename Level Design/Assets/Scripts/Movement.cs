using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public delegate void FloatsDelegate(float a, float b);
    public event FloatsDelegate OnRotation;
    public event FloatsDelegate OnWallRunRotation;

    float _currentSpeed, _normalSpeed, _sprintSpeed, _airSpeed, _xRotation, _mouseSensitivity, _jumpStrength, _maxVel, _groundDrag, _airDrag;
    Transform _playerTransform;
    Rigidbody _myRB;
    /*public LayerMask whatisWall;
    public float _maxWallrunTime, wallrunForce, currentWallRunTime, maxWallSpeed;
    bool isWallRunning = false;*/
    bool IsSliding = false;
    public float slideForce = 10f;



    public Movement(Transform transform, Rigidbody rigidbody, float speed, float mouseSensitivity, float jumpStrength, float gdrag, float adrag)
    {
        _playerTransform = transform;
        _myRB = rigidbody;
        _currentSpeed = speed;
        _normalSpeed = speed;
        _sprintSpeed = speed * 1.3f;
        _airSpeed = speed * 0.3f;
        _maxVel = 10;
        _mouseSensitivity = mouseSensitivity;
        _jumpStrength = jumpStrength;
        _groundDrag = gdrag;
        _airDrag = adrag;
    }

    public void Move(float horizontalInput, float verticalInput)
    {
        Vector3 direction = _playerTransform.forward * verticalInput + _playerTransform.right * horizontalInput;

        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        if (GroundedCheck())
        {
            _myRB.drag = _groundDrag;
            SpeedLimit(direction, _currentSpeed, _maxVel);
            //_myRB.AddForce(direction * _currentSpeed, ForceMode.Force);
        }
        else
        {
            _myRB.drag = _airDrag;
            SpeedLimit(direction, _airSpeed, _maxVel * 1.75f);
            //_myRB.AddForce(direction * _normalSpeed, ForceMode.Force);
        }
        

        //_myRB.MovePosition(_playerTransform.position + direction * _currentSpeed * Time.fixedDeltaTime);
        //_myRB.AddForce(direction * _currentSpeed * 5f, ForceMode.Force);
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
    void SpeedLimit(Vector3 dir, float spd, float limitValue)
    {
        float oldSpd = _myRB.velocity.sqrMagnitude;

        _myRB.AddForce(dir * spd, ForceMode.Force);

        if (_myRB.velocity.sqrMagnitude > oldSpd && oldSpd > limitValue)
        {
            _myRB.AddForce(-dir * spd, ForceMode.Force);
        }
    }

    public void Sprint()
    {
        _currentSpeed = _sprintSpeed;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        if (!IsSliding) _myRB.velocity = movement * _sprintSpeed;
    }

    public void StopSprint()
    {
        _currentSpeed = _normalSpeed;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        if (!IsSliding) _myRB.velocity = movement * _sprintSpeed;
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
        //_myRB.velocity = Vector3.zero;
        _myRB.AddForce(dir * strength * 0.9f);
    }

    public void Slide()
    {
        IsSliding = true;
        if (IsSliding)
        {
            _myRB.AddForce(Vector3.forward * slideForce);
        }
    }
    public void StopSlide()
    {
        IsSliding = false;
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
