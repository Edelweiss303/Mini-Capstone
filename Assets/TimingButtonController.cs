using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TimingButtonController : Singleton<TimingButtonController>
{
    public Text TimerText;
    public List<TimingButton> Buttons;

    public List<Sprite> PatternImages;
    public List<Sprite> BGImages;
    public string RadarScanSoundEffectName;
    public Image Pattern1;
    private Dictionary<Sprite, int> numberToColourMap = new Dictionary<Sprite, int>();
    private DateTime startTime;
    TimingButton selectedButton;

    // Start is called before the first frame update
    void Start()
    {
        Buttons = GetComponentsInChildren<TimingButton>().ToList();

        PatternImages = Resources.LoadAll<Sprite>("ButtonTiming/patterns").ToList();
        BGImages = Resources.LoadAll<Sprite>("ButtonTiming/colours").ToList();

        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        string minutes = (DateTime.Now - startTime).Minutes.ToString();
        string seconds = (DateTime.Now - startTime).Seconds.ToString();
        TimerText.text = (minutes)[minutes.Length-1] + ":" + seconds.PadLeft(2,'0');

    }
    
    public void ResetGame()
    {
        startTime = DateTime.Now;

        List<Sprite> availablePatterns = new List<Sprite>(PatternImages);
        List<Sprite> availableColours = new List<Sprite>(BGImages);
        List<Sprite> usedColours = new List<Sprite>();
        List<int> availableNumbers = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        numberToColourMap.Clear();
        Sprite currentSprite;
        int currentNumber;
        foreach (TimingButton button in Buttons)
        {
            currentSprite = availableColours[UnityEngine.Random.Range(0, availableColours.Count)];
            button.BGImage.sprite = currentSprite;
            availableColours.Remove(currentSprite);
            usedColours.Add(currentSprite);
            currentNumber = availableNumbers[UnityEngine.Random.Range(0, availableNumbers.Count)];
            numberToColourMap.Add(currentSprite, currentNumber);
            availableNumbers.Remove(currentNumber);
            currentSprite = availablePatterns[UnityEngine.Random.Range(0, availablePatterns.Count)];
            button.PatternImage.sprite = currentSprite;
            availablePatterns.Remove(currentSprite);
        }

        List<TimingButton> availableButtons = new List<TimingButton>(Buttons);
        selectedButton = availableButtons[UnityEngine.Random.Range(0, availableButtons.Count)];

        //Randomize who to send to
        bool aIsGunner = UnityEngine.Random.Range(0, 2) == 1;
        aIsGunner = true;
        string aPlayer = aIsGunner ? "g" : "p";
        string bPlayer = aIsGunner ? "p" : "g";

        string choosePatternMessage = aPlayer + ":MiniGameTBChoosePatterns:" + selectedButton.PatternImage.sprite.name;
        GameNetwork.Instance.ToPlayerQueue.Add(choosePatternMessage);

        string colourButtonMessage = bPlayer + ":MiniGameTBSetColours";
        foreach (TimingButton button in Buttons)
        {
            //Send colour image message
            colourButtonMessage += ":" + button.BGImage.sprite.name + ":" + numberToColourMap[button.BGImage.sprite].ToString();
        }
        GameNetwork.Instance.ToPlayerQueue.Add(colourButtonMessage);
    }

    public void TryClick(TimingButton inButton)
    {
        int correctNumber;
        TechnicianController.Instance.IncreaseHeat(1.0f);
        if (selectedButton == inButton)
        {
            correctNumber = numberToColourMap[selectedButton.BGImage.sprite];
            if (TimerText.text.Contains(correctNumber.ToString()))
            {
                GameNetwork.Instance.ToPlayerQueue.Add("p:PilotRadarScan");
                AudioManager.Instance.PlaySound(RadarScanSoundEffectName);
                TechnicianController.Instance.TimingButton_BackPress();
            }
        }

        
    }
}
