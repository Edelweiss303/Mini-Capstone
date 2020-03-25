using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ImageMatchGameController;

public class TechnicianMessenger : MonoBehaviour
{
    public static TechnicianMessenger Instance;
    public IconBehaviour IconMatchImage;

    private Dictionary<ImageColour, Dictionary<string,Sprite>> bgImages, mgImages, fgImages = new Dictionary<ImageColour, Dictionary<string,Sprite>>();

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        //Load all of the images into the image maps
        bgImages = getIconImages("Base");
        mgImages = getIconImages("Midground");
        fgImages = getIconImages("Foreground");
    }

    public void UpdateIcon(string[] iconDetailsMessage)
    {
        IconMatchImage.MidgroundImage.transform.rotation = Quaternion.identity;
        IconMatchImage.ForegroundImage.transform.rotation = Quaternion.identity;

        IconMatchImage.BGColour = getColourFromCode(iconDetailsMessage[3]);
        IconMatchImage.BackgroundImage.sprite = bgImages[IconMatchImage.BGColour][iconDetailsMessage[2]];

        IconMatchImage.MGColour = getColourFromCode(iconDetailsMessage[6]);
        IconMatchImage.MidgroundImage.sprite = mgImages[IconMatchImage.MGColour][iconDetailsMessage[4]];
        IconMatchImage.MidgroundOrientation = float.Parse(iconDetailsMessage[5]);

        IconMatchImage.FGColour = getColourFromCode(iconDetailsMessage[9]);
        IconMatchImage.ForegroundImage.sprite = fgImages[IconMatchImage.FGColour][iconDetailsMessage[7]];
        IconMatchImage.ForegroundOrientation = float.Parse(iconDetailsMessage[8]);

        IconMatchImage.MidgroundImage.transform.Rotate(0, 0, IconMatchImage.MidgroundOrientation);
        IconMatchImage.ForegroundImage.transform.Rotate(0, 0, IconMatchImage.ForegroundOrientation);
    }

    private ImageMatchGameController.ImageColour getColourFromCode(string code)
    {
        switch (code)
        {
            case "orange":
                return ImageMatchGameController.ImageColour.orange;
            case "blue":
                return ImageMatchGameController.ImageColour.blue;
            case "green":
                return ImageMatchGameController.ImageColour.green;
            default:
                return ImageMatchGameController.ImageColour.red;
        }
    }

    Dictionary<ImageColour, Dictionary<string, Sprite>> getIconImages(string folderName)
    {
        Dictionary<ImageColour, Dictionary<string, Sprite>> iconImages = new Dictionary<ImageColour, Dictionary<string, Sprite>>();

        foreach (ImageColour colour in ImageColour.GetValues(typeof(ImageColour)))
        {
            iconImages.Add(colour, new Dictionary<string,Sprite>());
        }

        List<Sprite> allImages = Resources.LoadAll<Sprite>(folderName).ToList();

        foreach (Sprite image in allImages)
        {
            foreach (ImageColour colour in ImageColour.GetValues(typeof(ImageColour)))
            {
                if (image.name.Contains(colour.ToString()))
                {
                    iconImages[colour].Add(image.name,image);
                }
            }
        }

        return iconImages;
    }
}
