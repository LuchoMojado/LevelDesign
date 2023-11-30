using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : Entity
{
    [SerializeField] float maxHp, _speed, _jumpStrength, _grappleRange, _hookSpeed, _propelStr, _climbSpeed, _wallCheckRange, _wallrunMinAngle, _minWallRunSpd, _gDrag, _aDrag, _wallRunSpeed;

    [Range(200, 1000), SerializeField]
    float _mouseSensitivity;
    Rigidbody _myRB;
    [SerializeField] Image _crosshair;

    public Movement movement;
    Inputs _inputs;
    WallrunController _wallrun;
    [field:SerializeField] public GrapplingHook _grapplingHook { private set; get; }
    [HideInInspector] public ConfigurableJoint joint;

    [SerializeField] Transform _leftRay, _rightRay;
    [HideInInspector] public RaycastHit hookHit;
    [SerializeField] PlayerCamera _camera;
    Transform _cameraTransform;
    public bool canGrapple, isWallRunning, isWallGrabbing;
    bool _wallingRight;
    [SerializeField] LayerMask _hookMask, _wallMask;
    //bool lisen = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _myRB = GetComponent<Rigidbody>();
        _cameraTransform = Camera.main.transform;

        movement = new Movement(transform, _myRB, _speed, _mouseSensitivity, _jumpStrength, _gDrag, _aDrag, _wallRunSpeed);
        _inputs = new Inputs(movement, this);
        _wallrun = new WallrunController(transform, _leftRay, _rightRay, _wallMask, _myRB, _wallCheckRange, _wallrunMinAngle, _minWallRunSpd);
    }

    private void Start()
    {
        EventManager.Subscribe("MakeSound", MakeSound);
        _hp = maxHp;
        _inputs.inputUpdate = _inputs.Unpaused;
    }

    void Update()
    {
        if (_inputs.inputUpdate != null)
            _inputs.inputUpdate();

        if (canGrapple)
            _crosshair.color = Color.white;
        else
            _crosshair.color = new Color(1, 1, 1, 0.2f);

        if (!_grapplingHook.shot)
        {
            if (Physics.Raycast(transform.position, _cameraTransform.forward, out hookHit, _grappleRange, _hookMask))
            {
                canGrapple = true;
            }
            else
            {
                canGrapple = false;
            }
        }
        else
        {
            canGrapple = false;
        }
        if (!isWallRunning && !isWallGrabbing)
        {
            if (!movement.GroundedCheck() && !_grapplingHook.grappled)
            {
                if (movement.isSprinting)
                {
                    if(_wallrun.WallCheckStart(true, out bool right, out float angle))
                    {
                        _wallrun.StartWall(_camera, right, true, angle);
                        isWallRunning = true;
                        WallStarted(right);
                    }
                }
                else if (_inputs.wallGrab)
                {
                    if (_wallrun.WallCheckStart(false, out bool right, out float angle))
                    {
                        _wallrun.StartWall(_camera, right, false, angle);
                        isWallGrabbing = true;
                        _wallingRight = right;
                        WallStarted(right);
                    }
                }
            }
        }
        else
        {
            if (!_wallrun.CheckWall(_wallingRight))
                WallFinished();
        }

        //MakeSound();

    }

    private void FixedUpdate()
    {
        _inputs.InputFixedUpdate();
    }

    /*public void HealUp(float heal)
    {
        _hp += heal;
    }*/

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

    public void Climb(bool up)
    {
        if (up && _grapplingHook.ChangeJointDistance(-_climbSpeed).limit > 0.5f)
            joint.linearLimit = _grapplingHook.ChangeJointDistance(-_climbSpeed);
        else if (_grapplingHook.ChangeJointDistance(_climbSpeed).limit < _grappleRange)
        {
            joint.linearLimit = _grapplingHook.ChangeJointDistance(_climbSpeed);
        }
    }

    public void WallStarted(bool right)
    {
        _wallingRight = right;
        movement.StartWall();
        movement.SetWallJump(right);
    }

    public void WallFinished()
    {
        _myRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        isWallRunning = false;
        isWallGrabbing = false;
        movement.StopWall();
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

    public void MakeSound(params object[] makingSound)
    {
        if((bool)makingSound[0])
        {
            EventManager.Trigger("ILisen", transform.position);
        }
    }

    
}

