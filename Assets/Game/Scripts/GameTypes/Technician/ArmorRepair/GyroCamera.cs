using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GyroCamera : MonoBehaviour
{
    public float horizontalRange = 33.0f;
    public float verticalRange = 29.0f;

    private Gyroscope gyro;
    private Vector3 rotationAmount;
    private Camera camera;
    void Awake()
    {
        gyro = Input.gyro;
        gyro.enabled = true;

        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        GyroInput();
        if(InputManager.Instance.inputMode == InputManager.InputMode.KeyAndMouse)
        {
            float vY = 0, vX = 0;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                vY++;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                vY--;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                vX++;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                vX--;
            }
            transform.Rotate(vY, vX, 0);
        }
    }

    private void GyroInput()
    {
        rotationAmount = gyro.rotationRateUnbiased ;
        rotationAmount.x *= -1;
        rotationAmount.y *= -1;
        
        transform.Rotate(rotationAmount);
    }
}
