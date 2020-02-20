using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigationController : MonoBehaviour
{
    public GameObject Crosshairs;
    public Button StartBtn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MenuNavigationUpdate();
    }

    void MenuNavigationUpdate()
    {
        if(Crosshairs)
        {
            Crosshairs.transform.position = InputManager.Instance.CursorLocation;

            if (InputManager.Instance.FireInput) //&& StartBtn.targetGraphic.rectTransform.rect.Contains(Crosshairs.transform.position))
            {
                StartBtn.onClick.Invoke();
            }
        }
    }
}
