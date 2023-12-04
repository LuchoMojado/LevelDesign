using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallrunMove : IMovementType
{

    Rigidbody _rb;
    Transform _playerTransform;
    float _drag, _speed, _fallSpeed;

    public WallrunMove(Rigidbody rb, Transform transform, float drag, float speed, float fallSpeed)
    {
        _rb = rb;
        _playerTransform = transform;
        _drag = drag;
        _speed = speed;
        _fallSpeed = fallSpeed;
    }

    public void MoveType()
    {
        _rb.AddForce(_playerTransform.forward * _speed * Time.deltaTime);
        _rb.drag = _drag;
        _rb.AddForce(-_playerTransform.up * _fallSpeed * Time.deltaTime);
    }
}
