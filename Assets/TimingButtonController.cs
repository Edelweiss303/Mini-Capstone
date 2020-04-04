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

    public Image Pattern1, Pattern2, Pattern3;
    public Image Failure1, Failure2, Failure3;
    public Image Progress1, Progress2, Progress3;

    public Color FailColour, ProgressColour, DefaultColour;

    private Dictionary<Sprite, int> numberToColourMap = new Dictionary<Sprite, int>();
    private DateTime startTime;
    private Dictionary<int, TimingButton> selectedButtons = new Dictionary<int, TimingButton>() { { 1, null }, { 2, null }, { 3, null } };



    int numberOfFailures = 0, numberOfSuccesses = 0;

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
        TimingButton currentButton = availableButtons[UnityEngine.Random.Range(0, availableButtons.Count)];
        selectedButtons[1] = currentButton;
        availableButtons.Remove(currentButton);

        currentButton = availableButtons[UnityEngine.Random.Range(0, availableButtons.Count)];
        selectedButtons[2] = currentButton;
        availableButtons.Remove(currentButton);

        currentButton = availableButtons[UnityEngine.Random.Range(0, availableButtons.Count)];
        selectedButtons[3] = currentButton;
        availableButtons.Remove(currentButton);

        //Randomize who to send to
        bool aIsGunner = UnityEngine.Random.Range(0, 2) == 1;
        aIsGunner = true;
        string aPlayer = aIsGunner ? "g" : "p";
        string bPlayer = aIsGunner ? "p" : "g";

        string choosePatternMessage = aPlayer + ":MiniGameTBChoosePatterns:1:" + selectedButtons[1].PatternImage.sprite.name +
                                            ":2:" + selectedButtons[2].PatternImage.sprite.name + 
                                            ":3:" + selectedButtons[3].PatternImage.sprite.name;
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
        bool validPress = false;
        int correctNumber;

        foreach (KeyValuePair<int,TimingButton> button in selectedButtons)
        {
            if (button.Value == inButton)
            {
                correctNumber = numberToColourMap[button.Value.BGImage.sprite];
                if (TimerText.text.Contains(correctNumber.ToString()))
                {
                    validPress = true;
                }
            }
        }

        if (!validPress)
        {
            numberOfFailures++;

            switch (numberOfFailures)
            {
                case 1:
                    Failure1.color = FailColour;
                    break;
                case 2:
                    Failure2.color = FailColour;
                    break;
                case 3:
                    Failure3.color = FailColour;
                    break;
            }
        }
        else
        {
            numberOfSuccesses++;

            switch (numberOfSuccesses)
            {
                case 1:
                    Progress1.color = ProgressColour;
                    break;
                case 2:
                    Progress2.color = ProgressColour;
                    break;
                case 3:
                    Progress3.color = ProgressColour;
                    break;
            }
        }
    }
}
