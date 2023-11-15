using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public delegate void FloatsDelegate(float a, float b);
    public event FloatsDelegate OnRotation;
    public event FloatsDelegate OnWallRunRotation;

    float _currentSpeed, _normalSpeed, _sprintSpeed, _airSpeed, _xRotation, _mouseSensitivity, _jumpStrength, _maxVel, _groundDrag, _airDrag, _wallRunSpeed;
    Transform _playerTransform;
    Rigidbody _myRB;
    /*public LayerMask whatisWall;
    public float _maxWallrunTime, wallrunForce, currentWallRunTime, maxWallSpeed;
    bool isWallRunning = false;*/
    public bool isSliding = false, isSprinting = false;
    bool _isWallRunning = false, _isWallGrabbing = false;
    public float slideForce = 30;
    bool _wallJumpRight;


    public Movement(Transform transform, Rigidbody rigidbody, float speed, float mouseSensitivity, float jumpStrength, float gdrag, float adrag, float wallSpd)
    {
        _playerTransform = transform;
        _myRB = rigidbody;
        _currentSpeed = speed;
        _normalSpeed = speed;
        _sprintSpeed = speed * 1.3f;
        _airSpeed = speed * 0.3f;
        _maxVel = 1;
        _mouseSensitivity = mouseSensitivity;
        _jumpStrength = jumpStrength;
        _groundDrag = gdrag;
        _airDrag = adrag;
        _wallRunSpeed = wallSpd;
    }

    public void Move(float horizontalInput, float verticalInput)
    {
        Vector3 direction = _playerTransform.forward * verticalInput + _playerTransform.right * horizontalInput;

        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        if (!_isWallRunning && !_isWallGrabbing)
        {
            if (GroundedCheck())
            {
                if (!isSliding)
                {
                    _myRB.drag = _groundDrag;
                    _myRB.AddForce(direction * _currentSpeed * Time.deltaTime, ForceMode.Force);
                }
                else
                {
                    _myRB.AddForce(direction * _normalSpeed * Time.deltaTime, ForceMode.Force);
                }
                
                //_myRB.AddForce(direction * _currentSpeed, ForceMode.Force);
            }
            else
            {
                _myRB.drag = _airDrag;
                _myRB.AddForce(direction * _airSpeed * Time.deltaTime, ForceMode.Force);
                //_myRB.AddForce(direction * _normalSpeed, ForceMode.Force);
            }
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
            Debug.Log("kkkkk");
        }
        else
        {
            OnWallRunRotation(_xRotation, verticalMouse);
            Debug.Log("yea");
        }
            
    }
    void SpeedLimit(Vector3 dir, float spd, float limitValue)
    {
        float oldSpd = _myRB.velocity.magnitude;
        
        _myRB.AddForce(dir * spd, ForceMode.Force);

        if (_myRB.velocity.magnitude > oldSpd && oldSpd > limitValue)
        {
            _myRB.AddForce(-dir * spd, ForceMode.Force);
        }
    }

    public void Sprint()
    {
        _currentSpeed = _sprintSpeed;
        isSprinting = true;
    }

    public void StopSprint()
    {
        _currentSpeed = _normalSpeed;
        isSprinting = false;
    }

    public bool Jump(bool grappled, out bool stopWallRun)
    {
        if (GroundedCheck())
        {
            _myRB.AddForce(Vector3.up * (_jumpStrength));
            stopWallRun = false;
            return false;
        }
        else if (_isWallRunning || _isWallGrabbing)
        {
            if (_wallJumpRight)
            {
                _myRB.AddForce((Vector3.up + -_playerTransform.right) * (_jumpStrength));
            }
            else
            {
                _myRB.AddForce((Vector3.up + _playerTransform.right) * (_jumpStrength));
            }
            stopWallRun = true;
            return false;
        }
        else if (grappled && _playerTransform.gameObject.GetComponent<Player>().hookHit.point.y > _playerTransform.position.y)
        {
            _myRB.AddForce(Vector3.up * (_jumpStrength * 1.25f));
            stopWallRun = false;
            return true;
        }
        else
        {
            stopWallRun = false;
            return false;
        }
    }

    public void SetWallJump(bool right)
    {
        _wallJumpRight = right;
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

    public void Slide(bool start)
    {
        if (start)
        {
            isSliding = true;
            _myRB.drag = 0.02f;
            _myRB.AddForce(Vector3.forward * slideForce);
        }
        else
        {
            isSliding = false;
        }
    }

    public void Walling(bool running)
    {
        if (running)
        {
            _myRB.AddForce(_playerTransform.forward * _wallRunSpeed * Time.deltaTime);
            _myRB.drag = _groundDrag;
        }
        else
        {
            _myRB.drag = _groundDrag * 2;
        }

        _myRB.AddForce(-_playerTransform.up * 0.5f);
    }
    public void StartWall(bool running)
    {
        if (running)
        {
            _myRB.AddForce(_playerTransform.forward * _wallRunSpeed);
            _isWallRunning = true;
        }
        else
        {
            _isWallGrabbing = true;
        }
        
        _myRB.useGravity = false;
    }
    public void StopWall()
    {
        _isWallGrabbing = false;
        _isWallRunning = false;
        _myRB.useGravity = true;
    }
}
