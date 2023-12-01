using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrabMove : IMovementType
{
    Rigidbody _rb;
    Transform _playerTransform;
    float _drag;

    public WallGrabMove(Rigidbody rb, Transform transform, float drag)
    {
        _rb = rb;
        _playerTransform = transform;
        _drag = drag;
    }

    public void MoveType()
    {
        _rb.drag = _drag;
        _rb.AddForce(-_playerTransform.up);
    }
}
