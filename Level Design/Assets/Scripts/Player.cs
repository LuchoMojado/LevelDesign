using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Player : Entity
{
    [SerializeField] float _speed, _jumpStrength;

    [Range(500, 1000), SerializeField]
    float _mouseSensitivity;

    Rigidbody _myRB;

    public Movement movement;
    Inputs _inputs;

    float _xRotation;
    Ray _ray;
    RaycastHit _rayHit;
    Transform _cameraTransform;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _myRB = GetComponent<Rigidbody>();
        _cameraTransform = Camera.main.transform;

        movement = new Movement(transform, _myRB, _speed, _mouseSensitivity, _jumpStrength);
        _inputs = new Inputs(movement, this);
    }

    private void Start()
    {
        _hp = maxHp;
        _inputs.inputUpdate = _inputs.Unpaused;
    }

    void Update()
    {
        if(_inputs.inputUpdate != null)
            _inputs.inputUpdate();
    }

    private void FixedUpdate()
    {
        _inputs.InputFixedUpdate();
    }

    public void HealUp(float heal)
    {
        _hp += heal;
    }

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);

        if (_hp <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _inputs.inputUpdate = null;
        //UIManager.instance.GameOver();
    }
}

