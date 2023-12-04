using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrabMove : IMovementType
{
    Rigidbody _rb;
    Transform _playerTransform;
    float _drag, _fallSpeed;

    public WallGrabMove(Rigidbody rb, Transform transform, float drag, float fallSpeed)
    {
        _rb = rb;
        _playerTransform = transform;
        _drag = drag;
        _fallSpeed = fallSpeed;
    }

    public void MoveType()
    {
        _rb.drag = _drag;
        _rb.AddForce(-_playerTransform.up * _fallSpeed * Time.deltaTime);
    }
}
