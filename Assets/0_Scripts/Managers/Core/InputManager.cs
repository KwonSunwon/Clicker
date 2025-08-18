using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
	KeyCode[] _monitoredKeys = new KeyCode[]
    {
	    KeyCode.W,
	    KeyCode.A,
	    KeyCode.S,
	    KeyCode.D,
	    KeyCode.Space,
	    KeyCode.LeftShift
    };

	public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;
	public Action<Vector2> MouseMoveAction = null;

	bool _pressed = false;
    float _pressedTime = 0;

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

		if (KeyAction != null)
			foreach (KeyCode key in _monitoredKeys)
			{
				if (Input.GetKeyUp(key))
				{
					KeyAction.Invoke();
					break;
				}
			}

		if (Input.anyKey && KeyAction != null)
				KeyAction.Invoke();

		

		if (MouseMoveAction != null)
		{
			Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			if (delta != Vector2.zero)
				MouseMoveAction.Invoke(delta);
		}

		if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_pressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed)
                {
                    if (Time.time < _pressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _pressed = false;
                _pressedTime = 0;
            }
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
		MouseMoveAction = null;
	}
}
