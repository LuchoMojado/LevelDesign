using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs
{
    public System.Action inputUpdate;
    public float _inputHorizontal { private set; get; }
    public float _inputVertical { private set; get; }
    float _inputMouseX, _inputMouseY;
    Movement _movement;
    Player _player;
    bool _jump;

    public Inputs(Movement movement, Player player)
    {
        _movement = movement;
        _player = player;
    }

    public void Unpaused()
    {
        _inputMouseX = Input.GetAxisRaw("Mouse X");

        _inputMouseY = Input.GetAxisRaw("Mouse Y");

        _inputHorizontal = Input.GetAxis("Horizontal");

        _inputVertical = Input.GetAxis("Vertical");

        _movement.Rotation(_inputMouseX, _inputMouseY, _player._isWallRunning);

        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //UIManager.instance.SetPauseMenu(true);
            inputUpdate = Paused;
        }*/

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (_player._canGrapple)
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

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (_inputVertical <= 0)
            {
                _movement.StopSprint();
                return;
            }
            _movement.Sprint();
           
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
            _movement.StopSprint();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jump = true;
        }

        if (Input.GetKey(KeyCode.LeftControl) && !_movement.isSliding)
        {
            _player.Slide();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
            _player.StopSlide();

        if (_player._isWallRunning && _inputVertical == 1)
        {
            _movement.Walling(true);
            if (_movement.GroundedCheck())
            {
                _player.StopWall();
            }
            else if(!_player.CheckWallRunSpeed())
            {
                _player.StopWall();
            }
        }
        else if (_player._isWallRunning)
        {
            _player.StopWall();
        }

        if (_player._isWallGrabbing)
        {
            _movement.Walling(false);
            if (_movement.GroundedCheck())
            {
                _player.StopWall();
            }
        }
    }

    public void Paused()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //UIManager.instance.SetPauseMenu(false);
            inputUpdate = Unpaused;
        }
    }

    public void InputFixedUpdate()
    {
        _movement.Move(_inputHorizontal, _inputVertical);

        if (_jump)
        {
            if (_movement.Jump(_player._grapplingHook.grappled, out bool stopWall))
            {
                _player.UseUngrapple();
            }
            if (stopWall)
            {
                _player.StopWall();
            }
            _jump = false;
        }
    }
}
