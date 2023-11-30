using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallrunController
{
    Transform _transform, _leftRay, _rightRay;
    Rigidbody _rb;
    LayerMask _mask;
    float _checkRange, _minAngle, _minSpeed;

    public WallrunController(Transform playerTransform, Transform leftRay, Transform rightRay, LayerMask mask, Rigidbody playerRigidbody, float range, float minAngle, float minSpeed)
    {
        _transform = playerTransform;
        _leftRay = leftRay;
        _rightRay = rightRay;
        _mask = mask;
        _rb = playerRigidbody;
        _checkRange = range;
        _minAngle = minAngle;
        _minSpeed = minSpeed;
    }

    public bool WallCheckStart(bool running, out bool right, out float angle)
    {
        bool leftSide = Physics.Raycast(_leftRay.position, _transform.forward, out RaycastHit leftWallHit, _checkRange, _mask);
        bool rightSide = Physics.Raycast(_rightRay.position, _transform.forward, out RaycastHit rightWallHit, _checkRange, _mask);

        if (rightSide)
        {
            right = true;
            angle = Vector3.Angle(_transform.forward, Vector3.Reflect(_transform.forward, rightWallHit.normal));
            if (running)
            {
                if (angle <= _minAngle && _rb.velocity.magnitude > _minSpeed)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        else if (leftSide)
        {
            right = false;
            angle = Vector3.Angle(_transform.forward, Vector3.Reflect(_transform.forward, leftWallHit.normal));
            if (running)
            {
                if (angle <= _minAngle && _rb.velocity.magnitude > _minSpeed)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        angle = default;
        right = default;
        return false;
    }

    public void StartWall(PlayerCamera camera, bool right, bool running, float angle)
    {
        float newAngle = angle * 0.5f;

        if (running)
        {
            if (right)
            {
                _transform.rotation = Quaternion.Euler(0, _transform.eulerAngles.y - newAngle, 0);
                camera.StartWall(right, newAngle);
            }
            else
            {
                _transform.rotation = Quaternion.Euler(0, _transform.eulerAngles.y + newAngle, 0);
                camera.StartWall(right, -newAngle);
            }
        }
        else
        {
            if (right)
            {
                _transform.rotation = Quaternion.Euler(0, _transform.eulerAngles.y - newAngle, 0);
            }
            else
            {
                _transform.rotation = Quaternion.Euler(0, _transform.eulerAngles.y + newAngle, 0);
            }
        }

        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public bool CheckWall(bool right)
    {
        Ray ray;

        ray = right ? new Ray(_transform.position, _transform.right) : new Ray(_transform.position, -_transform.right);

        if (right)
        {
            ray = new Ray(_transform.position, _transform.right);
        }
        else
        {
            ray = new Ray(_transform.position, -_transform.right);
        }

        return Physics.Raycast(ray, 1);
    }
}
