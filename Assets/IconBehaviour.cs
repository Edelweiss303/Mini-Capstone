using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconBehaviour : MonoBehaviour
{
    public Image BackgroundImage, MidgroundImage, ForegroundImage;
    public string BGName, MGName, FGName;
    public ImageMatchGameController.ImageColour BGColour, MGColour, FGColour;
    public float MidgroundOrientation, ForegroundOrientation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string ToString()
    {
        return BackgroundImage.sprite.name + ":" + MidgroundImage.sprite.name + MidgroundOrientation + ":" + ForegroundImage.sprite.name + ForegroundOrientation;
    }
}
