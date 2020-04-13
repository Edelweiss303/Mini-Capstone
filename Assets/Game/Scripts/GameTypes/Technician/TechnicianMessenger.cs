using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static ImageMatchGameController;

public class TechnicianMessenger : MonoBehaviour
{
    public static TechnicianMessenger Instance;
    public IconBehaviour IconMatchImage;
    public Text LockMessageText;
    public Image Pattern1Image;
    public Slider HeatSlider;
    public GameObject PatternsContainer, ColourMatchersContainer;
    public string OverheatedSoundEffectName, ReleaseHeatSoundEffectName;

    private Dictionary<string, Sprite> patternImages = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> colourImages = new Dictionary<string, Sprite>();
    private PlayerShootingBehaviour shootBehaviour;

    public List<ColourMatchButton> ColourMatchers = new List<ColourMatchButton>();

    public Dictionary<ImageMatchGameController.ImageLayer,
    Dictionary<ImageMatchGameController.ImageHue,
        Dictionary<ImageMatchGameController.ImageValue,
            Dictionary<string,Sprite>>>> matchingImages =
    new Dictionary<ImageMatchGameController.ImageLayer, Dictionary<ImageMatchGameController.ImageHue, Dictionary<ImageMatchGameController.ImageValue, Dictionary<string,Sprite>>>>();

    // Start is called before the first frame update
    void Start()
    {
        HeatSlider.maxValue = 100.0f;
        HeatSlider.value = 0.0f;
        if (Instance == null)
        {
            Instance = this;
        }

        if (GameNetwork.Instance.Type == GameNetwork.PlayerType.Gunner)
        {
            shootBehaviour = GunnerController.Instance.PlayerObject.GetComponent<PlayerBehaviour>().shootBehaviour;
        }

        List<Sprite> loadingPatternImages = Resources.LoadAll<Sprite>("ButtonTiming/patterns").ToList();
        List<Sprite> loadingColourImages = Resources.LoadAll<Sprite>("ButtonTiming/colours").ToList();

        getIconImages();

        foreach(Sprite patternImage in loadingPatternImages)
        {
            patternImages.Add(patternImage.name, patternImage);
        }


        foreach(Sprite colourImage in loadingColourImages)
        {
            colourImages.Add(colourImage.name, colourImage);
        }
    }

    public void UpdateIcon(string[] iconDetailsMessage)
    {
        ResetMessenger();
        IconMatchImage.gameObject.SetActive(true);
        
        IconMatchImage.MidgroundImage.transform.rotation = Quaternion.identity;
        IconMatchImage.ForegroundImage.transform.rotation = Quaternion.identity;

        IconMatchImage.BGColour = getColourFromCode(iconDetailsMessage[3]);
        IconMatchImage.BGValue = getValueFromCode(iconDetailsMessage[4]);
        IconMatchImage.BGName = iconDetailsMessage[2];
        IconMatchImage.BackgroundImage.sprite = matchingImages[ImageLayer.back][IconMatchImage.BGColour][IconMatchImage.BGValue][IconMatchImage.BGName];

        IconMatchImage.MGColour = getColourFromCode(iconDetailsMessage[7]);
        IconMatchImage.MGValue = getValueFromCode(iconDetailsMessage[8]);
        IconMatchImage.MGName = iconDetailsMessage[5];
        IconMatchImage.MidgroundImage.sprite = matchingImages[ImageLayer.mid][IconMatchImage.MGColour][IconMatchImage.MGValue][IconMatchImage.MGName];
        IconMatchImage.MidgroundOrientation = float.Parse(iconDetailsMessage[6]);

        IconMatchImage.FGColour = getColourFromCode(iconDetailsMessage[11]);
        IconMatchImage.FGValue = getValueFromCode(iconDetailsMessage[12]);
        IconMatchImage.FGName = iconDetailsMessage[9];
        IconMatchImage.ForegroundImage.sprite = matchingImages[ImageLayer.front][IconMatchImage.FGColour][IconMatchImage.FGValue][IconMatchImage.FGName];
        IconMatchImage.ForegroundOrientation = float.Parse(iconDetailsMessage[10]);

        IconMatchImage.MidgroundImage.transform.Rotate(0, 0, IconMatchImage.MidgroundOrientation);
        IconMatchImage.ForegroundImage.transform.Rotate(0, 0, IconMatchImage.ForegroundOrientation);
    }

    public void UpdateLockMessage(string lockMessage)
    {
        ResetMessenger();
        LockMessageText.gameObject.SetActive(true);
        LockMessageText.text = lockMessage;
    }

    public void ResetMessenger()
    {
        Debug.Log("Reset received.");
        IconMatchImage.gameObject.SetActive(false);
        LockMessageText.gameObject.SetActive(false);
        PatternsContainer.SetActive(false);
        ColourMatchersContainer.SetActive(false);
    }

    public void UpdatePatterns(string pattern1Name)
    {
        PatternsContainer.SetActive(true);
        Pattern1Image.sprite = patternImages[pattern1Name];
    }

    public void UpdateColours(string[] messageSegments)
    {
        ColourMatchersContainer.SetActive(true);
        List<ColourMatchButton> availableColourMatchers = new List<ColourMatchButton>(ColourMatchers);
        ColourMatchButton currentColourMatcher;

        for(int i = 2; i <= 21; i += 2)
        {
            currentColourMatcher = availableColourMatchers[UnityEngine.Random.Range(0, availableColourMatchers.Count)];
            currentColourMatcher.ColourImage.sprite = colourImages[messageSegments[i]];
            currentColourMatcher.ColourNumber.text = messageSegments[i + 1];
            availableColourMatchers.Remove(currentColourMatcher);
        }
    }

    private ImageMatchGameController.ImageHue getColourFromCode(string code)
    {
        switch (code)
        {
            case "r":
                return ImageMatchGameController.ImageHue.r;
            case "g":
                return ImageMatchGameController.ImageHue.g;
            default:
                return ImageMatchGameController.ImageHue.b;
        }
    }

    private ImageValue getValueFromCode(string code)
    {
        Debug.Log("Code:" + code);
        switch (code)
        {
            case "shade":
                return ImageValue.shade;
            case "tint":
                return ImageValue.tint;
            case "tone":
                return ImageValue.tone;
            default:
                return ImageValue.main;
        }
    }

    void getIconImages()
    {
        List<ImageLayer> layers = new List<ImageLayer>();
        List<ImageHue> hues = new List<ImageHue>();
        List<ImageValue> values = new List<ImageValue>();
        foreach (ImageLayer layer in Enum.GetValues(typeof(ImageLayer)))
        {
            layers.Add(layer);
            matchingImages.Add(layer, new Dictionary<ImageHue, Dictionary<ImageValue, Dictionary<string, Sprite>>>());
            foreach (ImageHue hue in Enum.GetValues(typeof(ImageHue)))
            {
                if (!hues.Contains(hue))
                {
                    hues.Add(hue);
                }
                matchingImages[layer].Add(hue, new Dictionary<ImageValue, Dictionary<string,Sprite>>());
                foreach (ImageValue value in Enum.GetValues(typeof(ImageValue)))
                {
                    if (!values.Contains(value))
                    {
                        values.Add(value);
                    }
                    matchingImages[layer][hue].Add(value, new Dictionary<string,Sprite>());
                }
            }
        }


        List<Sprite> allImages = Resources.LoadAll<Sprite>("ImageMatch").ToList();
        ImageLayer currentLayer;
        ImageHue currentHue;
        ImageValue currentValue;
        foreach (Sprite image in allImages)
        {
            string[] imageSegments = image.name.Split('_');
            if (layers.Any(l => l.ToString() == imageSegments[2]))
            {
                currentLayer = layers.Single(l => l.ToString() == imageSegments[2]);

                if (hues.Any(l => l.ToString() == imageSegments[0]))
                {
                    currentHue = hues.Single(l => l.ToString() == imageSegments[0]);

                    if (values.Any(l => l.ToString() == imageSegments[1]))
                    {
                        currentValue = values.Single(l => l.ToString() == imageSegments[1]);

                        matchingImages[currentLayer][currentHue][currentValue].Add(image.name,image);
                    }
                }
            }

        }
    }

    public void SetHeat(string[] messageSegments)
    {
        float newHeat = float.Parse(messageSegments[1]);
        switch (GameNetwork.Instance.Type)
        {
            case GameNetwork.PlayerType.Gunner:
                if(shootBehaviour == null)
                {
                    Debug.Log("This is broken.. :(");
                }
                if (shootBehaviour.IsOverheated && newHeat == 0)
                {
                    AudioManager.Instance.PlaySound(ReleaseHeatSoundEffectName);
                    shootBehaviour.IsOverheated = false;
                }
                break;
            case GameNetwork.PlayerType.Pilot:
                if (PilotController.Instance.PlayerController.IsOverheated && newHeat == 0)
                {
                    AudioManager.Instance.PlaySound(ReleaseHeatSoundEffectName);
                    PilotController.Instance.PlayerController.IsOverheated = false;
                }
                break;
        }

        HeatSlider.value = newHeat;
    }

    public void SetOverheated()
    {
        if(GameNetwork.Instance.Type == GameNetwork.PlayerType.Gunner)
        {
            if (shootBehaviour)
            {
                AudioManager.Instance.PlaySound(OverheatedSoundEffectName);
                shootBehaviour.IsOverheated = true;
            }
        }
        else if(GameNetwork.Instance.Type == GameNetwork.PlayerType.Pilot)
        {
            AudioManager.Instance.PlaySound(OverheatedSoundEffectName);
            PilotController.Instance.PlayerController.IsOverheated = true;
        }
    }



}
