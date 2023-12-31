using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs
{
    public System.Action inputUpdate;
    public float _inputHorizontal { get; private set; }
    public float _inputVertical { get; private set; }
    float _inputMouseX, _inputMouseY;
    Movement _movement;
    Player _player;
    bool _jump;
    public bool wallGrab;
    public IMovementType normalMove, airMove;

    public Inputs(Movement movement, Player player)
    {
        _movement = movement;
        _player = player;
    }
    public void InputStart()
    {
        inputUpdate = Unpaused;
        GameManager.instance.Pause = Unpause;
    }
    public void Unpaused()
    {
        _inputMouseX = Input.GetAxisRaw("Mouse X");

        _inputMouseY = Input.GetAxisRaw("Mouse Y");

        _inputHorizontal = Input.GetAxis("Horizontal");

        _inputVertical = Input.GetAxis("Vertical");

        if (_player.isWallRunning || _player.isWallGrabbing)
        {
            _movement.Rotation(_inputMouseX, _inputMouseY, true);
        }
        else
        {
            _movement.Rotation(_inputMouseX, _inputMouseY, false);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.PauseGame();
            inputUpdate = Paused;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T");
            //GameManager.instance.LoadGame();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (_player.canGrapple)
                _player.UseGrapple();
        }

        if (_player._grapplingHook.grappled)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _player.UseUngrapple();
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _player.PropelToHook();
            }
            if (Input.GetKey(KeyCode.R))
            {
                _player.Climb(true);
            }
            else if (Input.GetKey(KeyCode.F))
            {
                _player.Climb(false);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && !_movement.isSliding)
        {
            if (_inputVertical <= 0)
            {
                _movement.StopSprint();
                EventManager.Trigger("MakeSound", false);
                return;
            }
            EventManager.Trigger("MakeSound", true);
            _movement.Sprint();
           
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            EventManager.Trigger("MakeSound", false);
            _movement.StopSprint();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("yea");
            _jump = true;
        }

        if (Input.GetKey(KeyCode.LeftControl) && !_movement.isSprinting)
        {
            wallGrab = true;

            if (!_movement.isSliding)
            {
                _player.Slide();
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            wallGrab = false;

            _player.StopSlide();
        }

        if (_player.isWallRunning && _inputVertical == 1)
        {
            if (_movement.GroundedCheck())
            {
                _player.WallFinished();
            }
            else if(!_player.CheckWallRunSpeed())
            {
                _player.WallFinished();
            }
        }
        else if (_player.isWallRunning)
        {
            _player.WallFinished();
        }

        if (_player.isWallGrabbing)
        {
            if (_movement.GroundedCheck() || !wallGrab)
            {
                _player.WallFinished();
            }
        }
    }

    public void Paused()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.UnpauseGame();
            inputUpdate = Unpaused;
        }
    }

    public void InputFixedUpdate()
    {
        if (_jump)
        {
            Debug.Log("si");
            if (_movement.Jump(_player._grapplingHook.grappled, out bool stopWall))
            {
                _player.UseUngrapple();
            }
            if (stopWall)
            {
                _player.WallFinished();
            }
            _jump = false;
        }
    }
    public void Unpause()
    {
        inputUpdate = Unpaused;
    }
}
