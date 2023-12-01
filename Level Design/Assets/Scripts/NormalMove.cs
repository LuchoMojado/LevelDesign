using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMove : IMovementType
{
    Rigidbody _rb;
    Transform _playerTransform;
    Movement _movement;
    Inputs _inputs;

    public NormalMove(Rigidbody rb, Transform transform, Inputs input, Movement movement)
    {
        _rb = rb;
        _playerTransform = transform;
        _inputs = input;
        _movement = movement;
    }

    public void MoveType()
    {
        Vector3 direction = _playerTransform.forward * _inputs._inputVertical + _playerTransform.right * _inputs._inputHorizontal;

        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        _rb.drag = _movement.currentDrag;
        _rb.AddForce(direction * _movement.currentSpeed * Time.deltaTime, ForceMode.Force);
    }
}
