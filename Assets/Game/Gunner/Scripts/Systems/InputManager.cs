using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
//using UnityEngine.tvOS;


class InputManager : Singleton<InputManager>
{
    public enum InputMode
    {
        PC, AppleTV, AndroidTablet, Null
    }
    public InputMode inputMode = InputMode.Null;

    // public Vector3 CursorLocation = Vector3.zero;
    public Vector3 CursorMovement = Vector3.zero;
    public bool FireInput = false;
    public bool Reloading = false;
    public bool Escape = false;
    public float AppleTVReloadingThreshold = 1.0f;
    public float ReloadingCooldown = 5.0f;

    private float lastReloadingTime = -1.0f;
    private Vector3 lastCursorPosition = Vector3.zero;
    private Quaternion remoteCalibration;

    public void Start()
    {
        if (Application.platform == RuntimePlatform.tvOS)
        {
            inputMode = InputMode.AppleTV;
            //Remote.allowExitToHome = false;
            //Remote.touchesEnabled = true;

            Vector3 accelerationSnapshot = Input.acceleration;

            Quaternion rotateQuaternion = Quaternion.FromToRotation(
                new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);

            remoteCalibration = Quaternion.Inverse(rotateQuaternion);
            lastCursorPosition = Input.gyro.rotationRate;
        }
        else
        {
            inputMode = InputMode.PC;
            lastCursorPosition = Input.mousePosition;
        }
            
    }

    public void Update()
    {
        InputUpdate();
    }

    public void InputUpdate()
    {
        if (inputMode == InputMode.PC)
        {

            //CursorLocation = Input.mousePosition;
            FireInput = Input.GetMouseButtonDown(0);
            CursorMovement = Input.mousePosition - lastCursorPosition;

            //if (CursorMovement.sqrMagnitude > 1)
            //{
            //    CursorMovement.Normalize();
            //}

            lastCursorPosition = Input.mousePosition;
            //FireInput = Input.GetKeyDown(KeyCode.Space);
            Escape = Input.GetKeyDown(KeyCode.Escape);

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
            CursorMovement = new Vector3(-Input.gyro.rotationRate.z, Input.gyro.rotationRate.y, 0);

            FireInput = Input.GetButtonDown("Submit");
            Escape = Input.GetButtonDown("Pause");

            if (Reloading)
            {
                lastReloadingTime += Time.deltaTime;
                if(lastReloadingTime > ReloadingCooldown)
                {
                    Reloading = false;
                    lastReloadingTime = -1.0f;
                }
            }
        }

    }
}
