using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImageMatchGameController : MonoBehaviour
{
    public enum ImageColour
    {
        red, green, blue, orange
    }
    private Dictionary<ImageColour, List<Sprite>> bgImages, mgImages, fgImages = new Dictionary<ImageColour, List<Sprite>>();
    private List<float> imageOrientations = new List<float>() { 0, 90, 180, 270 };
    private List<string> usedIcons;

    public List<IconBehaviour> IconGrid;
    

    // Start is called before the first frame update
    void Start()
    {
        //Load all of the images into the image maps
        bgImages = getIconImages("Base");
        mgImages = getIconImages("Midground");
        fgImages = getIconImages("Foreground");

        startNewGrid();
    }

    Dictionary<ImageColour, List<Sprite>> getIconImages(string folderName)
    {
        Dictionary<ImageColour, List<Sprite>> iconImages = new Dictionary<ImageColour, List<Sprite>>();

        foreach(ImageColour colour in ImageColour.GetValues(typeof(ImageColour)))
        {
            iconImages.Add(colour, new List<Sprite>());
        }

        List<Sprite> allImages = Resources.LoadAll<Sprite>(folderName).ToList();

        foreach(Sprite image in allImages)
        {
            foreach (ImageColour colour in ImageColour.GetValues(typeof(ImageColour)))
            {
                if (image.name.Contains(colour.ToString()))
                {
                    iconImages[colour].Add(image);
                }
            }
        }

        return iconImages;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void startNewGrid()
    {
        bool validIconFound = false;
        List<ImageColour> currentlyAvailableColours;
        usedIcons = new List<string>();

        ImageColour tempColour;
        Sprite tempSprite;
        float tempOrientation;

        //Create a randomized and unique set of icons
        foreach(IconBehaviour icon in IconGrid)
        {
            validIconFound = false;
            
            while (!validIconFound)
            {
                currentlyAvailableColours = new List<ImageColour>() { ImageColour.red, ImageColour.orange, ImageColour.green, ImageColour.blue };

                tempColour = currentlyAvailableColours[Random.Range(0, currentlyAvailableColours.Count())];
                icon.BGColour = tempColour;

                tempSprite = bgImages[icon.BGColour][Random.Range(0, bgImages[icon.BGColour].Count())];
                icon.BackgroundImage.sprite = tempSprite;
                tempOrientation = imageOrientations[Random.Range(0, imageOrientations.Count())];
                icon.BackgroundOrientation = tempOrientation;

                currentlyAvailableColours.Remove(icon.BGColour);

                icon.MGColour = currentlyAvailableColours[Random.Range(0, currentlyAvailableColours.Count())];
                icon.MidgroundImage.sprite = mgImages[icon.MGColour][Random.Range(0, mgImages[icon.MGColour].Count())];
                icon.MidgroundOrientation = imageOrientations[Random.Range(0, imageOrientations.Count())];

                currentlyAvailableColours.Remove(icon.MGColour);

                icon.FGColour = currentlyAvailableColours[Random.Range(0, currentlyAvailableColours.Count())];
                icon.ForegroundImage.sprite = fgImages[icon.FGColour][Random.Range(0, fgImages[icon.FGColour].Count())];
                icon.ForegroundOrientation = imageOrientations[Random.Range(0, imageOrientations.Count())];

                if (!usedIcons.Contains(icon.ToString()))
                {
                    usedIcons.Add(icon.ToString());
                    validIconFound = true;
                    icon.BackgroundImage.transform.Rotate(0, 0, icon.BackgroundOrientation);
                    icon.MidgroundImage.transform.Rotate(0, 0, icon.MidgroundOrientation);
                    icon.ForegroundImage.transform.Rotate(0, 0, icon.ForegroundOrientation);
                }
            }
        }
    }

    
}
