using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputDetection : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKey)
        {
            InputManager.Instance.inputMode = InputManager.InputMode.KeyAndMouse;
            MainMenuButtons.Instance.InputTypeAccepted();
        }

        if(Input.touches.Length > 0)
        {
            if(Application.platform == RuntimePlatform.Android)
            {
                InputManager.Instance.inputMode = InputManager.InputMode.AndroidTablet;
                MainMenuButtons.Instance.InputTypeAccepted();
            }
            else if(Application.platform == RuntimePlatform.tvOS)
            {
                InputManager.Instance.inputMode = InputManager.InputMode.AppleTV;
                MainMenuButtons.Instance.InputTypeAccepted();
            }
        }
    }
}
