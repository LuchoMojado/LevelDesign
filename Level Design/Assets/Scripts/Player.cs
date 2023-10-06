using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : Entity
{
    [SerializeField] float _speed, _jumpStrength, _grappleRange, _hookSpeed, _propelStr, _climbSpeed, _wallCheckRange, _wallrunMinAngle, _minWallRunSpd, _gDrag, _aDrag;

    [Range(500, 1000), SerializeField]
    float _mouseSensitivity;

    Rigidbody _myRB;
    [SerializeField] Image _crosshair;
    Coroutine _wallRunCR;

    public Movement movement;
    Inputs _inputs;
    [field:SerializeField] public GrapplingHook _grapplingHook { private set; get; }
    [HideInInspector] public ConfigurableJoint joint;

    float _xRotation;
    float _wallCD;
    [SerializeField] Transform _leftRay, _rightRay;
    [HideInInspector] public RaycastHit hookHit, leftWallHit, rightWallHit;
    [SerializeField] PlayerCamera _camera;
    Transform _cameraTransform;
    public bool _canGrapple { private set; get; }
    public bool _isWallRunning { private set; get; }
    public bool _isWallGrabbing { private set; get; }
    bool _wallingRight;

    //bool lisen = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _myRB = GetComponent<Rigidbody>();
        _cameraTransform = Camera.main.transform;

        movement = new Movement(transform, _myRB, _speed, _mouseSensitivity, _jumpStrength, _gDrag, _aDrag);
        _inputs = new Inputs(movement, this);
    }

    private void Start()
    {
        EventManager.Subscribe("MakeSound", MakeSound);
        _hp = maxHp;
        _inputs.inputUpdate = _inputs.Unpaused;
    }

    void Update()
    {
        if (Physics.Raycast(_leftRay.position, transform.forward, out leftWallHit, _wallCheckRange))
        {
            var angle = Vector3.Angle(transform.forward, Vector3.Reflect(transform.forward, leftWallHit.normal));
        }
        /*if (Physics.Raycast(_rightRay.position, transform.forward, out rightWallHit, _wallCheckRange))
        {
            var angle = Vector3.Angle(transform.forward, Vector3.Reflect(transform.forward, rightWallHit.normal));
            print(angle);
        }*/

        if (_inputs.inputUpdate != null)
            _inputs.inputUpdate();

        if (_canGrapple)
            _crosshair.color = Color.white;
        else
            _crosshair.color = new Color(1, 1, 1, 0.2f);

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
        if (!_isWallRunning || !_isWallGrabbing)
        {
            if (_wallCD <= 0)
            {
                if (!movement.GroundedCheck() && !_grapplingHook.grappled)
                {
                    bool left = Physics.Raycast(_leftRay.position, transform.forward, out leftWallHit, _wallCheckRange);
                    bool right = Physics.Raycast(_rightRay.position, transform.forward, out rightWallHit, _wallCheckRange);

                    if (right)
                    {
                        var angle = Vector3.Angle(transform.forward, Vector3.Reflect(transform.forward, rightWallHit.normal));
                        if (angle <= _wallrunMinAngle && _inputs._inputVertical == 1 && _inputs._inputHorizontal > 0 && _myRB.velocity.magnitude > _minWallRunSpd && _myRB.velocity.y < 5)
                        {
                            StartWall(true, true, angle);
                        }
                        else if (_inputs._inputHorizontal > 0)
                        {
                            print("yea");
                            StartWall(true, false, angle);
                        }
                    }
                    else if (left)
                    {
                        var angle = Vector3.Angle(transform.forward, Vector3.Reflect(transform.forward, leftWallHit.normal));
                        if (angle <= _wallrunMinAngle && _inputs._inputVertical == 1 && _inputs._inputHorizontal < 0 && _myRB.velocity.magnitude > _minWallRunSpd && _myRB.velocity.y < 5)
                        {
                            StartWall(false, true, angle);
                        }
                        else if (_inputs._inputHorizontal < 0)
                        {
                            print("yea");
                            StartWall(false, false, angle);
                        }
                    }
                }
            }
            else
            {
                _wallCD -= Time.deltaTime;
            }
        }
        else
        {
            if (!CheckWall(_wallingRight))
                StopWall();
        }

        //MakeSound();

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
        if (yea && _grapplingHook.ChangeJointDistance(-_climbSpeed).limit > 0.5f)
            joint.linearLimit = _grapplingHook.ChangeJointDistance(-_climbSpeed);
        else
            if (_grapplingHook.ChangeJointDistance(_climbSpeed).limit < _grappleRange)
        {
            joint.linearLimit = _grapplingHook.ChangeJointDistance(_climbSpeed);
        }
    }

    public void StartWall(bool right, bool running, float angle)
    {
        float newAngle = angle * 0.5f;

        if (running)
        {
            _isWallRunning = true;
        }
        else
        {
            _isWallGrabbing = true;
        }
        
        movement.StartWall(running);

        if (right)
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y - newAngle, 0);
            _camera.StartWall(right, newAngle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + newAngle, 0);
            _camera.StartWall(right, -newAngle);
        }

        _wallingRight = right;
        movement.SetWallJump(right);
        _myRB.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void StopWall()
    {
        _myRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _isWallRunning = false;
        movement.StopWall();
        _wallCD = 0.2f;
    }

    public void Slide()
    {
        if (movement.GroundedCheck())
        {
            movement.Slide(true);
            _camera.ChangeCameraY(-0.4f);
        }
    }

    public void StopSlide()
    {
        movement.Slide(false);
        _camera.ChangeCameraY(0);
    }

    public bool CheckWallRunSpeed()
    {
        if (_minWallRunSpd > _myRB.velocity.sqrMagnitude)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CheckWall(bool right)
    {
        Ray ray;

        if (right)
        {
            ray = new Ray(transform.position, transform.right);
        }
        else
        {
            ray = new Ray(transform.position, -transform.right);
        }

        return Physics.Raycast(ray, 1);
    }

    public void MakeSound(params object[] makingSound)
    {
        if((bool)makingSound[0])
        {
            EventManager.Trigger("ILisen", transform.position);
        }
    }
}

