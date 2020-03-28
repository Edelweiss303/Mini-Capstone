using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeySquare : MonoBehaviour
{
    public Text CodeText;
    public Image KeyImage; 

    // Start is called before the first frame update
    void Start()
    {
        KeyImage = GetComponent<Image>();
        CodeText = GetComponentInChildren<Text>();
    }
}
