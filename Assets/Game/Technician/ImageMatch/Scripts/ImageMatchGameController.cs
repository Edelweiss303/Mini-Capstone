using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImageMatchGameController : Singleton<ImageMatchGameController>
{
    public enum ImageColour
    {
        red, green, blue, orange
    }
    private Dictionary<ImageColour, List<Sprite>> bgImages, mgImages, fgImages = new Dictionary<ImageColour, List<Sprite>>();
    private List<float> imageOrientations = new List<float>() { 0, 90, 180, 270 };
    private List<string> usedIcons;

    public int IconSize = 64;
    public int IconColumns = 10;
    public int IconRows = 10;

    public List<IconBehaviour> IconGrid;
    private IconBehaviour iconToMatch;
    public GameObject IconPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //Load all of the images into the image maps
        bgImages = getIconImages("Base");
        mgImages = getIconImages("Midground");
        fgImages = getIconImages("Foreground");

        resetGame();
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

    void startNewGrid(int size, int columns, int rows)
    {
        for(int i = IconGrid.Count -1; i >= 0; i--)
        {
            Destroy(IconGrid[i]);
        }
        IconGrid.Clear();

        GameObject temp;
        Vector3 currentOffset;
        for(int c = 0; c < columns; c++)
        {
            for(int r = 0; r < rows; r++)
            {
                currentOffset = new Vector3(transform.position.x + r * IconSize, transform.position.y + -c * IconSize);
                temp = Instantiate(IconPrefab, currentOffset, Quaternion.identity, transform);
                IconGrid.Add(temp.GetComponent<IconBehaviour>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            selectIconToMatch();
            initialized = true;
        }

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
                        if (selectedIcon.ToString() == iconToMatch.ToString())
                        {
                            resetGrid();
                            selectIconToMatch();
                        }
                    }
                }
            }

        }
    }

    void resetGrid()
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
                icon.BGName = icon.BackgroundImage.sprite.name;
                currentlyAvailableColours.Remove(icon.BGColour);

                icon.MGColour = currentlyAvailableColours[Random.Range(0, currentlyAvailableColours.Count())];
                icon.MidgroundImage.sprite = mgImages[icon.MGColour][Random.Range(0, mgImages[icon.MGColour].Count())];
                icon.MGName = icon.MidgroundImage.sprite.name;
                icon.MidgroundOrientation = imageOrientations[Random.Range(0, imageOrientations.Count())];

                currentlyAvailableColours.Remove(icon.MGColour);

                icon.FGColour = currentlyAvailableColours[Random.Range(0, currentlyAvailableColours.Count())];
                icon.ForegroundImage.sprite = fgImages[icon.FGColour][Random.Range(0, fgImages[icon.FGColour].Count())];
                icon.FGName = icon.ForegroundImage.sprite.name;
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
        iconToMatch = IconGrid[Random.Range(0, IconGrid.Count())];
        string iconSetupMessage = "g:ChooseIcon:" + iconToMatch.BGName + ":" + iconToMatch.BGColour.ToString() +":" +
                                    iconToMatch.MGName + ":"  + iconToMatch.MidgroundOrientation.ToString() + ":" + iconToMatch.MGColour + ":" +
                                    iconToMatch.FGName + ":"  + iconToMatch.ForegroundOrientation.ToString() + ":" + iconToMatch.FGColour;
        GameNetwork.Instance.ToPlayerQueue.Add(iconSetupMessage);

    }

    public void resetGame()
    {
        startNewGrid(IconSize, IconColumns, IconRows);
        resetGrid();
    }
}
