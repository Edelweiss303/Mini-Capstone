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
    public IconBehaviour IconToMatch;

    // Start is called before the first frame update
    void Start()
    {
        //Load all of the images into the image maps
        bgImages = getIconImages("Base");
        mgImages = getIconImages("Midground");
        fgImages = getIconImages("Foreground");

        startNewGrid();
        selectIconToMatch();
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
        IconBehaviour selectedIcon;
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                var temp = Input.mousePosition;
                temp.z = 10;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(temp);
                Vector2 mousePos2D = new Vector2(temp.x, temp.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    selectedIcon = hit.transform.gameObject.GetComponent<IconBehaviour>();
                    if (selectedIcon)
                    {
                        if (selectedIcon.ToString() == IconToMatch.ToString())
                        {
                            startNewGrid();
                            selectIconToMatch();
                        }
                    }
                }
            }

        }
    }

    void startNewGrid()
    {
        bool validIconFound = false;
        List<ImageColour> currentlyAvailableColours;
        usedIcons = new List<string>();

        ImageColour tempColour;
        Sprite tempSprite;

        //Create a randomized and unique set of icons
        foreach(IconBehaviour icon in IconGrid)
        {
            validIconFound = false;
            icon.MidgroundImage.transform.rotation = Quaternion.identity;
            icon.ForegroundImage.transform.rotation = Quaternion.identity;
            while (!validIconFound)
            {
                currentlyAvailableColours = new List<ImageColour>() { ImageColour.red, ImageColour.orange, ImageColour.green, ImageColour.blue };

                tempColour = currentlyAvailableColours[Random.Range(0, currentlyAvailableColours.Count())];
                icon.BGColour = tempColour;

                tempSprite = bgImages[icon.BGColour][Random.Range(0, bgImages[icon.BGColour].Count())];
                icon.BackgroundImage.sprite = tempSprite;
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
                    icon.MidgroundImage.transform.Rotate(0, 0, icon.MidgroundOrientation);
                    icon.ForegroundImage.transform.Rotate(0, 0, icon.ForegroundOrientation);
                }
            }
        }
    }

    void selectIconToMatch()
    {
        IconBehaviour randomIcon = IconGrid[Random.Range(0, IconGrid.Count())];
        IconToMatch.BackgroundImage.sprite = randomIcon.BackgroundImage.sprite;
        IconToMatch.BGColour = randomIcon.BGColour;

        IconToMatch.MidgroundImage.sprite = randomIcon.MidgroundImage.sprite;
        IconToMatch.MidgroundOrientation = randomIcon.MidgroundOrientation;
        IconToMatch.MGColour = randomIcon.MGColour;

        IconToMatch.ForegroundImage.sprite = randomIcon.ForegroundImage.sprite;
        IconToMatch.ForegroundOrientation = randomIcon.ForegroundOrientation;
        IconToMatch.FGColour = randomIcon.FGColour;

        IconToMatch.MidgroundImage.transform.rotation = Quaternion.identity;
        IconToMatch.ForegroundImage.transform.rotation = Quaternion.identity;

        IconToMatch.MidgroundImage.transform.Rotate(0, 0, IconToMatch.MidgroundOrientation);
        IconToMatch.ForegroundImage.transform.Rotate(0, 0, IconToMatch.ForegroundOrientation);
    }
    
}
