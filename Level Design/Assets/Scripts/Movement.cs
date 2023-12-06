using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public delegate void FloatsDelegate(float a, float b);
    public event FloatsDelegate OnRotation;
    public event FloatsDelegate OnWallRunRotation;

    float _normalSpeed, _sprintSpeed, _crouchSpeed, _xRotation, _mouseSensitivity, _jumpStrength, _maxVel, _groundDrag, _airDrag, _slideDrag, _wallRunSpeed;
    Transform _playerTransform;
    Rigidbody _myRB;
    /*public LayerMask whatisWall;
    public float _maxWallrunTime, wallrunForce, currentWallRunTime, maxWallSpeed;
    bool isWallRunning = false;*/
    public bool isSliding = false, isSprinting = false;
    bool _isWalling = false;
    public float slideForce = 30, currentSpeed, currentDrag, currentGroundDrag, currentGroundSpeed;
    bool _wallJumpRight;
    IMovementType _moveType;
    LayerMask _groundedLayer;

    public Movement(Transform transform, Rigidbody rigidbody, float speed, float sprintSpeed, float crouchSpeed, float mouseSensitivity, float jumpStrength, float gdrag, float adrag, float sDrag, float wallSpd, LayerMask groundedMask)
    {
        _playerTransform = transform;
        _myRB = rigidbody;
        currentSpeed = speed;
        _normalSpeed = speed;
        _sprintSpeed = sprintSpeed;
        _crouchSpeed = crouchSpeed;
        _mouseSensitivity = mouseSensitivity;
        _jumpStrength = jumpStrength;
        _groundDrag = gdrag;
        _slideDrag = sDrag;
        _groundedLayer = groundedMask;
    }

    public void Move()
    {
        _moveType.MoveType();
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
        {
            OnWallRunRotation(_xRotation, verticalMouse);
        }
            
    }

    public void Sprint()
    {
        currentGroundSpeed = _sprintSpeed;
        isSprinting = true;
    }

    public void StopSprint()
    {
        currentGroundSpeed = _normalSpeed;
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
        else if (_isWalling)
        {
            if (_wallJumpRight)
            {
                _myRB.AddForce((Vector3.up * 1.5f + -_playerTransform.right) * (_jumpStrength));
            }
            else
            {
                _myRB.AddForce((Vector3.up * 1.5f + _playerTransform.right) * (_jumpStrength));
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
        bool grounded = Physics.SphereCast(groundedRay, 0.5f, 1.1f, _groundedLayer);

        return grounded;
    }

    public void MoveToHook(Vector3 dir, float strength)
    {
        //_myRB.velocity = Vector3.zero;
        _myRB.AddForce(dir.normalized * strength * 0.9f);
    }

    public void Slide(bool start)
    {
        if (start)
        {
            isSliding = true;
            currentGroundDrag = _slideDrag;
            currentGroundSpeed = _normalSpeed * 0.5f;
        }
        else
        {
            isSliding = false;
            currentGroundDrag = _groundDrag;
            currentGroundSpeed = _normalSpeed;
        }
    }

    public void StartWall()
    {
        _isWalling = true;
        _myRB.useGravity = false;
    }
    public void StopWall()
    {
        _isWalling = false;
        _myRB.useGravity = true;
    }

    public void ChangeMoveType(IMovementType type)
    {
        _moveType = type;
    }
}
