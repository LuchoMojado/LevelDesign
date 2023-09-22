using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Player : Entity
{
    [SerializeField] float _speed, _jumpStrength, _grappleRange, _hookSpeed, _propelStr, _climbSpeed, _wallCheckRange, _wallrunMinAngle;
    
    [Range(500, 1000), SerializeField]
    float _mouseSensitivity;

    Rigidbody _myRB;

    public Movement movement;
    Inputs _inputs;
    [field:SerializeField] public GrapplingHook _grapplingHook { private set; get; }
    [HideInInspector] public ConfigurableJoint joint;

    float _xRotation;
    [SerializeField] Transform _leftRay, _rightRay;
    [HideInInspector] public RaycastHit hookHit, wallHit;
    [SerializeField] PlayerCamera _camera;
    Transform _cameraTransform;
    public bool _canGrapple { private set; get; }
    public bool _isWallRunning { private set; get; }

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

        if (!_grapplingHook.shot)
        {
            if (Physics.Raycast(transform.position, _cameraTransform.forward, out hookHit, _grappleRange))
            {
                _canGrapple = true;
            }
            else
            {
                _canGrapple = false;
            }
        }
        else
        {
            _canGrapple = false;
        }

        if (!movement.GroundedCheck() && !_isWallRunning)
        {
            if (Physics.Raycast(_leftRay.position, transform.forward, out wallHit, _wallCheckRange) || Physics.Raycast(_rightRay.position, transform.forward, out wallHit, _wallCheckRange))
            {
                if (Vector3.Angle(transform.forward, Vector3.Reflect(transform.forward, wallHit.normal)) >= _wallrunMinAngle /*&& chequeo de velocidad*/)
                {
                    if (wallHit.point.x > transform.position.x)
                    {
                        StartWallRun(true, wallHit);
                    }
                    else
                    {
                        StartWallRun(false, wallHit);
                    }
                }
                else
                {
                    // quedarse agarrado
                }
            }
        }
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

    public void UseGrapple()
    {
        StartCoroutine(_grapplingHook.Grapple(_hookSpeed, hookHit, gameObject));
    }

    public void UseUngrapple()
    {
        StartCoroutine(_grapplingHook.Ungrapple(_hookSpeed));
    }

    public void PropelToHook()
    {
        UseUngrapple();
        movement.MoveToHook(hookHit.point - transform.position, _propelStr);
    }

    public void Climb(bool yea)
    {
        if (yea)
            joint.linearLimit = _grapplingHook.ChangeJointDistance(-_climbSpeed);
        else
            if (_grapplingHook.ChangeJointDistance(_climbSpeed).limit < _grappleRange)
        {
            joint.linearLimit = _grapplingHook.ChangeJointDistance(_climbSpeed);
        }
    }

    public void StartWallRun(bool right, RaycastHit hit)
    {
        _camera.CameraTilt(right);
        /*if (right)
        {

        }*/
    }
}

