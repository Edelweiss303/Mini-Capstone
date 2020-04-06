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

    public enum Direction
    {
        up, down, left, right
    }

    public Vector3 CursorMovement = Vector3.zero;
    public Vector3 DirectionalInput = Vector3.zero;

    public Dictionary<Direction, bool> DirectionalPresses = new Dictionary<Direction, bool>() { { Direction.up, false }, { Direction.down, false }, { Direction.left, false }, { Direction.right, false } };
    public bool FireInput = false;
    public bool Escape = false;
    public bool Swiping = false;
    public float AppleTVReloadingThreshold = 1.0f;
    public float ReloadingCooldown = 5.0f;
    public bool DoubleCenterTap = false;
    public float CenterTapTimingThreshold = 0.2f;
    public float CenterTapDistanceThreshold = 5.0f;
    public float NewTouchThreshold = 0.05f;
    public float SwipingThreshold = 10.0f;

    private float timeSinceLastCenterTap = 0.0f;
    private Vector3 lastCursorPosition = Vector3.zero;
    private Quaternion remoteCalibration;
    private Vector2 centerOfScreen;


    public void Start()
    {
        if (Application.platform == RuntimePlatform.tvOS)
        {
            Vector3 accelerationSnapshot = Input.acceleration;

            Quaternion rotateQuaternion = Quaternion.FromToRotation(
                new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);

            remoteCalibration = Quaternion.Inverse(rotateQuaternion);
            lastCursorPosition = Input.gyro.rotationRate;
            centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
        }
        else
        {
            //inputMode = InputMode.PC;
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
            #region PC
            FireInput = Input.GetMouseButtonDown(0);
            CursorMovement = Input.mousePosition - lastCursorPosition;

            lastCursorPosition = Input.mousePosition;
            Escape = Input.GetKeyDown(KeyCode.Escape);
            Swiping = Input.GetKeyDown(KeyCode.E);
            DirectionalInput = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                DirectionalInput.y++;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                DirectionalInput.y--;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                DirectionalInput.x--;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                DirectionalInput.x++;
            }

            DirectionalPresses[Direction.up] = Input.GetKeyDown(KeyCode.W);
            DirectionalPresses[Direction.down] = Input.GetKeyDown(KeyCode.S);
            DirectionalPresses[Direction.left] = Input.GetKeyDown(KeyCode.A);
            DirectionalPresses[Direction.right] = Input.GetKeyDown(KeyCode.D);
            #endregion

        }
        else if (inputMode == InputMode.AppleTV)
        {
            CursorMovement = new Vector3(-Input.gyro.rotationRate.z, Input.gyro.rotationRate.y, 0);

            FireInput = Input.GetButtonDown("Submit");
            Escape = Input.GetButtonDown("Pause");
            timeSinceLastCenterTap += Time.deltaTime;

            if(Input.touches.Count() == 1)
            {
                GunnerController.Instance.InputTest.text = "";
                Swiping = Input.touches[0].deltaPosition.magnitude > SwipingThreshold;
                Vector2 currentTouchPosition = Input.touches[0].position;
                GunnerController.Instance.InputTest.text += "TouchSpeed: " + Input.touches[0].deltaPosition.magnitude + System.Environment.NewLine;
                GunnerController.Instance.InputTest.text += "CurrentTouchPosition: " + currentTouchPosition.x + ", " + currentTouchPosition.y + System.Environment.NewLine;

                bool isNewTouch = timeSinceLastCenterTap > NewTouchThreshold;
                bool hasBeenTooLongSinceLastTouch = timeSinceLastCenterTap > CenterTapTimingThreshold;
                bool isACenterTouch = (currentTouchPosition - centerOfScreen).magnitude < CenterTapDistanceThreshold;

                DoubleCenterTap = isNewTouch && !hasBeenTooLongSinceLastTouch && isACenterTouch;

                if (isNewTouch && isACenterTouch)
                {
                    timeSinceLastCenterTap = 0.0f;
                }
            }
            else
            {
                DoubleCenterTap = false;
            }
        }
    }
}
