using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.tvOS;


class InputManager : Singleton<InputManager>
{
    public enum InputMode
    {
        Mouse, AppleTV
    }
    public InputMode inputMode = InputMode.AppleTV;

    public Vector3 CursorLocation = Vector3.zero;
    public bool FireInput = false;

    public void Start()
    {
        if(inputMode == InputMode.AppleTV)
        {
            Remote.touchesEnabled = true;
        }
    }

    public void Update()
    {
        if(inputMode == InputMode.Mouse)
        {
            CursorLocation = Input.mousePosition;
            FireInput = Input.GetMouseButtonDown(0);
        }
        else if(inputMode == InputMode.AppleTV)
        {
            CursorLocation += Input.acceleration;
            FireInput = Input.GetButtonDown("A");
        }

            
            
    }
}
