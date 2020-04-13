using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImageMatchGameController : Singleton<ImageMatchGameController>
{
    public enum ImageHue
    {
        r, g , b
    }
    public enum ImageValue
    {
        main, shade, tone, tint
    }
    public enum ImageLayer
    {
        back, mid, front
    }

    public Dictionary<ImageLayer, Dictionary<ImageHue, Dictionary<ImageValue, List<Sprite>>>> matchingImages = new Dictionary<ImageLayer, Dictionary<ImageHue, Dictionary<ImageValue, List<Sprite>>>>();
    private List<ImageLayer> layers = new List<ImageLayer>();
    private List<ImageHue> hues = new List<ImageHue>();
    private List<ImageValue> values = new List<ImageValue>();
    private List<float> imageOrientations = new List<float>() { 0, 90, 180, 270 };
    private List<string> usedIcons;
    private bool imagesInitialized = false;
    private ImageHue selectedPowerupColour;

    public string PowerupSoundEffectName;

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
        if (!imagesInitialized)
        {
            getIconImages();
        }


        //resetGame();
    }

    void getIconImages()
    {
        foreach(ImageLayer layer in Enum.GetValues(typeof(ImageLayer)))
        {
            layers.Add(layer);
            matchingImages.Add(layer, new Dictionary<ImageHue, Dictionary<ImageValue, List<Sprite>>>());
            foreach(ImageHue hue in Enum.GetValues(typeof(ImageHue)))
            {
                if (!hues.Contains(hue))
                {
                    hues.Add(hue);
                }
                
                matchingImages[layer].Add(hue, new Dictionary<ImageValue, List<Sprite>>());
                foreach(ImageValue value in Enum.GetValues(typeof(ImageValue)))
                {
                    if (!values.Contains(value))
                    {
                        values.Add(value);
                    }
                    
                    matchingImages[layer][hue].Add(value, new List<Sprite>());
                }
            }
        }


        List<Sprite> allImages = Resources.LoadAll<Sprite>("ImageMatch").ToList();
        ImageLayer currentLayer;
        ImageHue currentHue;
        ImageValue currentValue;
        foreach(Sprite image in allImages)
        {
            string[] imageSegments = image.name.Split('_');
            if(layers.Any(l => l.ToString() == imageSegments[2]))
            {
                currentLayer = layers.Single(l => l.ToString() == imageSegments[2]);

                if(hues.Any(l => l.ToString() == imageSegments[0]))
                {
                    currentHue = hues.Single(l => l.ToString() == imageSegments[0]);

                    if(values.Any(l => l.ToString() == imageSegments[1]))
                    {
                        currentValue = values.Single(l => l.ToString() == imageSegments[1]);

                        matchingImages[currentLayer][currentHue][currentValue].Add(image);
                    }
                }
            }

        }
        imagesInitialized = true;
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
                    TechnicianController.Instance.IncreaseHeat(2.0f);
                    if (selectedIcon)
                    {
                        if (selectedIcon.ToString() == iconToMatch.ToString())
                        {
                            GameNetwork.Instance.ToPlayerQueue.Add("g:GunnerSetPowerup:" + selectedPowerupColour.ToString());
                            AudioManager.Instance.PlaySound(PowerupSoundEffectName);
                            TechnicianController.Instance.ImageMatch_BackPress();
                        }
                        else
                        {
                            AudioManager.Instance.PlaySound("Technician_Error");
                            TechnicianController.Instance.TakeDamage(0.5f);
                        }
                    }
                }
            }

        }
    }

    void resetGrid()
    {
        if (!imagesInitialized)
        {
            getIconImages();
        }

        bool validIconFound = false;
        List<ImageHue> currentlyAvailableColours = new List<ImageHue>(hues);
        ImageHue bgHue = currentlyAvailableColours[UnityEngine.Random.Range(0, currentlyAvailableColours.Count)];
        currentlyAvailableColours.Remove(bgHue);
        ImageHue mgHue = currentlyAvailableColours[UnityEngine.Random.Range(0, currentlyAvailableColours.Count)];
        currentlyAvailableColours.Remove(mgHue);
        ImageHue fgHue = currentlyAvailableColours[UnityEngine.Random.Range(0, currentlyAvailableColours.Count)];
        currentlyAvailableColours.Remove(fgHue);


        usedIcons = new List<string>();
        Sprite tempSprite;

        //Create a randomized and unique set of icons
        foreach(IconBehaviour icon in IconGrid)
        {
            validIconFound = false;
            icon.MidgroundImage.transform.rotation = Quaternion.identity;
            icon.ForegroundImage.transform.rotation = Quaternion.identity;
            while (!validIconFound)
            {
                icon.BGColour = bgHue;
                icon.BGValue = values[UnityEngine.Random.Range(0, values.Count)];
                tempSprite = matchingImages[ImageLayer.back][icon.BGColour][icon.BGValue][UnityEngine.Random.Range(0, matchingImages[ImageLayer.back][icon.BGColour][icon.BGValue].Count)];
                icon.BackgroundImage.sprite = tempSprite;
                icon.BGName = icon.BackgroundImage.sprite.name;

                icon.MGColour = mgHue;
                icon.MGValue = values[UnityEngine.Random.Range(0, values.Count)];
                tempSprite = matchingImages[ImageLayer.mid][icon.MGColour][icon.MGValue][UnityEngine.Random.Range(0, matchingImages[ImageLayer.mid][icon.MGColour][icon.MGValue].Count)];
                icon.MidgroundImage.sprite = tempSprite;
                icon.MGName = icon.MidgroundImage.sprite.name;
                icon.MidgroundOrientation = imageOrientations[UnityEngine.Random.Range(0, imageOrientations.Count())];


                icon.FGColour = fgHue;
                icon.FGValue = values[UnityEngine.Random.Range(0, values.Count)];

                tempSprite = matchingImages[ImageLayer.front][icon.FGColour][icon.FGValue][UnityEngine.Random.Range(0, matchingImages[ImageLayer.front][icon.FGColour][icon.FGValue].Count)];
                icon.ForegroundImage.sprite = tempSprite;
                icon.FGName = icon.ForegroundImage.sprite.name;
                icon.ForegroundOrientation = imageOrientations[UnityEngine.Random.Range(0, imageOrientations.Count())];

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
        iconToMatch = IconGrid[UnityEngine.Random.Range(0, IconGrid.Count())];
        string iconSetupMessage = "g:MiniGameIMChooseIcon:" + iconToMatch.BGName + ":" + iconToMatch.BGColour.ToString() + ":" + iconToMatch.BGValue.ToString() + ":" +
                                    iconToMatch.MGName + ":"  + iconToMatch.MidgroundOrientation.ToString() + ":" + iconToMatch.MGColour + ":" +  iconToMatch.MGValue.ToString() + ":" +
                                    iconToMatch.FGName + ":"  + iconToMatch.ForegroundOrientation.ToString() + ":" + iconToMatch.FGColour + ":"  + iconToMatch.FGValue;
        GameNetwork.Instance.ToPlayerQueue.Add(iconSetupMessage);

    }

    public void resetGame(ImageHue inHue)
    {
        selectedPowerupColour = inHue;
        startNewGrid(IconSize, IconColumns, IconRows);
        resetGrid();
        selectIconToMatch();
    }
}
