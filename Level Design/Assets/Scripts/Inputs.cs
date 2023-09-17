using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs
{
    public System.Action inputUpdate;
    float _inputHorizontal, _inputVertical;
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

        _movement.Rotation(_inputMouseX, _inputMouseY);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //UIManager.instance.SetPauseMenu(true);
            inputUpdate = Paused;
        }

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
            _movement.Jump();
            _jump = false;
        }
    }
}
