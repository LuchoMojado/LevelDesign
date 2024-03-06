using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : Entity, IPlaySound
{
    [SerializeField] float maxHp, _speed, _sprintSpeed, _crouchSpeed, _airSpeed, _jumpStrength, _grappleRange, _hookSpeed, _propelStr, _climbSpeed, _wallCheckRange, _wallrunMinAngle, _minWallRunSpd, _gDrag, _aDrag, _sDrag, _wallRunSpeed, _baseKnockbackStr, _knockbackIncreaseRate, _knockbackY;

    [Range(200, 1000), SerializeField]
    public AudioSource audioSource;
    public AudioClip audioclip;
    float _mouseSensitivity;
    Rigidbody _myRB;
    CapsuleCollider _myCol;
    [SerializeField] Image _crosshair;

    public Movement movement;
    Inputs _inputs;
    WallrunController _wallrun;
    [field:SerializeField] public GrapplingHook _grapplingHook { private set; get; }
    [HideInInspector] public ConfigurableJoint joint;

    [SerializeField] Transform _leftRay, _rightRay;
    [HideInInspector] public RaycastHit hookHit;
    [SerializeField] PlayerCamera _camera;
    public Transform _cameraTransform;
    [HideInInspector] public bool canGrapple, isWallRunning, isWallGrabbing;
    bool _wallingRight;
    [SerializeField] LayerMask _hookMask, _wallMask,_EnemyMask, _groundMask;
    //bool lisen = false;

    int _hitsReceived;
    bool _loading;

    protected override void Awake()
    {
        base.Awake();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _myRB = GetComponent<Rigidbody>();
        _myCol = GetComponent<CapsuleCollider>();
        _cameraTransform = Camera.main.transform;

        movement = new Movement(transform, _myRB, _speed, _sprintSpeed, _crouchSpeed, _mouseSensitivity, _jumpStrength, _gDrag, _aDrag, _sDrag, _wallRunSpeed, _groundMask);
        _inputs = new Inputs(movement, this);
        _wallrun = new WallrunController(transform, _leftRay, _rightRay, _wallMask, _myRB, _wallCheckRange, _wallrunMinAngle, _minWallRunSpd);
        movement.ChangeMoveType(new NormalMove(_myRB, transform, _inputs, movement));
        movement.currentGroundDrag = _gDrag;
        movement.currentGroundSpeed = _speed;
    }

    private void Start()
    {
        _inputs.InputStart();
        EventManager.Subscribe("MakeSound", MakeSound);
        _hp = maxHp;
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
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hookHit, _grappleRange, _hookMask))
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

        if (_grapplingHook.grappled && _grapplingHook.grappledTo.layer != 8 && _grapplingHook.grappledTo.layer != 12)
        {
            UseUngrapple();
        }

        if (!isWallRunning && !isWallGrabbing)
        {
            if (!movement.GroundedCheck())
            {
                movement.ChangeMoveType(new NormalMove(_myRB, transform, _inputs, movement));
                movement.currentDrag = _aDrag;
                movement.currentSpeed = _airSpeed;

                if (!_grapplingHook.grappled)
                {
                    if (movement.isSprinting)
                    {
                        if (_wallrun.WallCheckStart(true, out bool right, out float angle))
                        {
                            _wallrun.StartWall(_camera, right, true, angle);
                            isWallRunning = true;
                            WallStarted(right);
                            movement.ChangeMoveType(new WallrunMove(_myRB, transform, _gDrag, _wallRunSpeed, 150));
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
                            movement.ChangeMoveType(new WallGrabMove(_myRB, transform, _gDrag, 100));
                        }
                    }
                }
                else
                {
                    movement.ChangeMoveType(new NormalMove(_myRB, transform, _inputs, movement));
                }
            }
            else
            {
                movement.ChangeMoveType(new NormalMove(_myRB, transform, _inputs, movement));
                movement.currentDrag = movement.currentGroundDrag;
                movement.currentSpeed = movement.currentGroundSpeed;
            }
        }
        else
        {
            if (!_wallrun.CheckWall(_wallingRight))
            {
                WallFinished();
                movement.ChangeMoveType(new NormalMove(_myRB, transform, _inputs, movement));
                movement.currentDrag = _aDrag;
            }
                
        }

        //MakeSound();

    }

    private void FixedUpdate()
    {
        _inputs.InputFixedUpdate();

        movement.Move();
    }

    /*public void HealUp(float heal)
    {
        _hp += heal;
    }*/

    public override void Die()
    {
        GameManager.instance.LoadGame2();
        _hitsReceived = 0;
        //Time.timeScale = 0;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        //_inputs.inputUpdate = null;
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
        if (up)
            joint.linearLimit = _grapplingHook.ChangeJointDistance(-_climbSpeed, _grappleRange);
        else
            joint.linearLimit = _grapplingHook.ChangeJointDistance(_climbSpeed, _grappleRange);
    }

    public void WallStarted(bool right)
    {
        PlaySound(audioclip);
        _wallingRight = right;
        movement.StartWall();
        movement.SetWallJump(right);
    }

    public void WallFinished()
    {
        StopSound(audioclip);
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
            _camera.ChangeCameraY(0.25f);
            _myCol.height = 1;
            _myCol.center = new Vector3(0, -0.5f, 0);
        }
    }

    public void StopSlide()
    {
        movement.Slide(false);
        _camera.ChangeCameraY(0);
        _myCol.height = 2;
        _myCol.center = Vector3.zero;
    }
    public Collider[] colliders;
    public void MakeSound(params object[] makingSound)
    {
        if((bool)makingSound[0])
        {
            EventManager.Trigger("ILisen", transform.position);
            colliders = Physics.OverlapSphere(transform.position, 10);
            if(colliders!=null)
            {
                foreach (var item in colliders)
                {
                    item.TryGetComponent<IHear>(out IHear Hear);
                    if (Hear != null)
                    {
                        Hear.ChasePlayer(transform.position);
                        //EventManager.Trigger("ILisen", transform.position);
                    }
                        
                }
            }
        }
        
    }

    public bool CheckWallRunSpeed()
    {
        return _myRB.velocity.magnitude > _minWallRunSpd;
    }
    public override void Save()
    {
        if (_loading)
            return;
        _mementoState.Rec(transform.position, transform.rotation);
    }

    public override void Load()
    {
        //if(_mementoState.IsRemember())
        //{
        //    var data = _mementoState.Remember();
        //    //Pongo en el array la pos que se donde lo puse lo que quiero
        //    transform.position = (Vector3)data.parameters[0];
        //    transform.rotation = (Quaternion)data.parameters[1];
        //}
        if (_mementoState.IsRemember())
        {
            StartCoroutine(CoroutineLoad());
        }
    }

    IEnumerator CoroutineLoad()
    {
        var WaitForSeconds = new WaitForSeconds(0.01f);
        _loading = true;
        _myRB.constraints = RigidbodyConstraints.FreezeAll;
        while (_mementoState.IsRemember())
        {
            var data = _mementoState.Remember();
            //_loading = true;
            //Pongo en el array la pos que se donde lo puse lo que quiero
            transform.position = (Vector3)data.parameters[0];
            transform.rotation = (Quaternion)data.parameters[1];

            yield return WaitForSeconds;
        }
        _myRB.constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ;
        _loading = false;
        Save();
    }

    public void Knockback(float hazardX, float hazardZ)
    {
        if (_grapplingHook.grappled)
        {
            UseUngrapple();
        }

        _myRB.velocity = Vector3.zero;

        var dir = new Vector3(transform.position.x - hazardX, 0, transform.position.z - hazardZ).normalized;

        _myRB.AddForce((dir + Vector3.up * _knockbackY) * (_baseKnockbackStr + _knockbackIncreaseRate * _hitsReceived));

        _hitsReceived++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13)
        {
            // takedamage
            var colPoint = other.ClosestPoint(transform.position);
            Knockback(colPoint.x, colPoint.z);
        }

        if (other.gameObject.layer == 15)
        {
            print("ganaste");
        }
        if (other.gameObject.layer == 16)
        {
            // takedamage
            Die();
            //GameManager.instance.Respawn();
        }
    }
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    public void StopSound(AudioClip clip)
    {
        audioSource.Stop();
    }
}

