using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickFunctions : MonoBehaviour
{
    public Button[] buttons;
    public Text displayText;

    private void Start()
    {
        foreach(Button b in buttons)
        {
            b.onClick.AddListener(delegate { DisplayButtonText(b.GetComponentInChildren<Text>().text); });
        }
    }

    public void DisplayButtonText(string s)
    {
        displayText.text = s;
    }
    
}
