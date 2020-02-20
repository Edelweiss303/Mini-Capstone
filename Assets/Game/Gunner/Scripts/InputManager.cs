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
    public InputMode inputMode = InputMode.Mouse;

    public Vector3 CursorLocation = Vector3.zero;
    public bool FireInput = false;
    public bool Reloading = false;
    public float AppleTVReloadingThreshold = 1.0f;
    public float ReloadingCooldown = 5.0f;

    private float lastReloadingTime = -1.0f;

    public void Start()
    {
        if(inputMode == InputMode.AppleTV)
        {
            Remote.touchesEnabled = true;
        }
    }

    public void Update()
    {
        InputUpdate();
    }

    public void InputUpdate()
    {
        if (inputMode == InputMode.Mouse)
        {

            CursorLocation = Input.mousePosition;
            FireInput = Input.GetMouseButtonDown(0);

            if (Reloading)
            {
                lastReloadingTime += Time.deltaTime;
                if (lastReloadingTime > ReloadingCooldown)
                {
                    Reloading = false;
                    lastReloadingTime = -1.0f;
                }
            }
            else if (lastReloadingTime == -1.0f)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    lastReloadingTime = 0.0f;
                    Reloading = true;
                }
            }

        }
        else if (inputMode == InputMode.AppleTV)
        {
            if (Input.touches.Count() > 0)
            {
                CursorLocation.x += Input.touches[0].deltaPosition.x;
                CursorLocation.y += Input.touches[0].deltaPosition.y;
            }

            FireInput = Input.GetButtonDown("A");

            if (Reloading)
            {
                lastReloadingTime += Time.deltaTime;
                if(lastReloadingTime > ReloadingCooldown)
                {
                    Reloading = false;
                    lastReloadingTime = -1.0f;
                }
            }
            else if (lastReloadingTime == -1.0f)
            {
                if (Input.gyro.userAcceleration.magnitude > AppleTVReloadingThreshold)
                {
                    lastReloadingTime = 0.0f;
                    Reloading = true;
                }
            }
        }
    }
}
