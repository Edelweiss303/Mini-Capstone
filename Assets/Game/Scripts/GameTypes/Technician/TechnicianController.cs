﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ImageMatchGameController;

public class TechnicianController : Singleton<TechnicianController>
{
    public GameObject ImageMatchControllerObject, LockRotationControllerObject, ArmourRepairControllerObject, FrontPageObject, TimingButtonControllerObject, SelectPowerupColourObject;
    public GameObject ImageMatchInstructionsObject, LockRotationInstructionsObject, TimingButtonInstructionsObject;
    public GameObject GameOverPanel;
    public GameObject background;
    public Slider HeatSlider;
    public float MaxHeat = 100.0f;
    public float CurrentHeat = 0.0f;
    public string ReleaseHeatSoundEffectName, OverheatedSoundEffectName;
    public HealthBarBehaviour HealthBar;
    public Text ScoreText;
    public PlayerScreen PlayerViewScreen;

    private ImageMatchGameController imageGameController;
    private LockRotationGameController lockGameController;
    private TimingButtonController timingGameController;
    private bool Setup = false;

    private void Start()
    {
        HeatSlider.maxValue = MaxHeat;
        HeatSlider.value = CurrentHeat;
        imageGameController = ImageMatchControllerObject.GetComponent<ImageMatchGameController>();
        lockGameController = LockRotationControllerObject.GetComponent<LockRotationGameController>();
        timingGameController = TimingButtonControllerObject.GetComponent<TimingButtonController>();
    }

    private void Update()
    {
        if (!Setup)
        {
            ImageMatchControllerObject.SetActive(false);
            LockRotationControllerObject.SetActive(false);
            Setup = true;
        }
    }

    public void IncreaseHeat(float amountToIncrease)
    {
        if(CurrentHeat < MaxHeat)
        {
            CurrentHeat += amountToIncrease;
            HeatSlider.value = CurrentHeat;

            //Send update on heat to all other systems
            GameNetwork.Instance.BroadcastQueue.Add("TechnicianHeat:" + CurrentHeat);

            if (CurrentHeat >= MaxHeat)
            {
                //Ping all other games to freeze
                GameNetwork.Instance.BroadcastQueue.Add("TechnicianOverheated:" + CurrentHeat);
                AudioManager.Instance.PlaySound(OverheatedSoundEffectName);
            }
        }

    }

    public void ResetHeat()
    {
        CurrentHeat = 0.0f;
        HeatSlider.value = CurrentHeat;

        //Send update on heat to all other systems
        GameNetwork.Instance.BroadcastQueue.Add("TechnicianHeat:" + CurrentHeat);
        AudioManager.Instance.PlaySound(ReleaseHeatSoundEffectName);
        GameNetwork.Instance.BroadcastMessage("TechnicianMessengerReset");
    }

    public void FrontPage_ImageMatchPress()
    {
        FrontPageObject.SetActive(false);
        SelectPowerupColourObject.SetActive(true);
        GameNetwork.Instance.BroadcastMessage("TechnicianMessengerReset");
    }

    public void FrontPage_LockRotatePress()
    {
        FrontPageObject.SetActive(false);
        LockRotationControllerObject.SetActive(true);
        GameNetwork.Instance.BroadcastMessage("TechnicianMessengerReset");
        lockGameController.resetGame();

    }

    public void FrontPage_TimingButtonPress()
    {
        FrontPageObject.SetActive(false);
        TimingButtonControllerObject.SetActive(true);
        timingGameController.ResetGame();
        GameNetwork.Instance.BroadcastMessage("TechnicianMessengerReset");
    }


    public void FrontPage_PatchArmourButtonPress()
    {
        FrontPageObject.SetActive(false);
        background.SetActive(false);
        Camera.main.GetComponent<GyroCamera>().enabled = true; 
        Camera.main.GetComponent<TouchInput>().enabled = true;
        ArmourRepairControllerObject.SetActive(true);
        GameNetwork.Instance.BroadcastMessage("TechnicianMessengerReset");
    }

    public void ImageMatch_BackPress()
    {
        FrontPageObject.SetActive(true);
        ImageMatchControllerObject.SetActive(false);
        SelectPowerupColourObject.SetActive(false);
        GameNetwork.Instance.BroadcastMessage("TechnicianMessengerReset");
    }

    public void LockRotate_BackPress()
    {
        FrontPageObject.SetActive(true);
        LockRotationControllerObject.SetActive(false);
        GameNetwork.Instance.BroadcastMessage("TechnicianMessengerReset");
    }

    public void TimingButton_BackPress()
    {
        FrontPageObject.SetActive(true);
        TimingButtonControllerObject.SetActive(false);
        GameNetwork.Instance.BroadcastMessage("TechnicianMessengerReset");
    }

    public void ArmourRepair_BackPress()
    {
        FrontPageObject.SetActive(true);
        background.SetActive(true);
        Camera.main.GetComponent<GyroCamera>().enabled = false;
        Camera.main.GetComponent<TouchInput>().enabled = false;
        ArmourRepairControllerObject.SetActive(false);
        GameNetwork.Instance.BroadcastMessage("TechnicianMessengerReset");
    }

    public void SelectPowerupPage_SelectPowerup(string colour)
    {
        SelectPowerupColourObject.SetActive(false);
        ImageMatchControllerObject.SetActive(true);

        ImageHue selectedHue;

        switch (colour)
        {
            case "red":
                selectedHue = ImageHue.r;
                break;
            case "blue":
                selectedHue = ImageHue.b;
                break;
            case "green":
                selectedHue = ImageHue.g;
                break;
            default:
                return;
        }
        imageGameController.resetGame(selectedHue);
        GameNetwork.Instance.BroadcastMessage("TechnicianMessengerReset");
    }

    public void SetHealth(float newHealth)
    {
        HealthBar.Health = newHealth / 15.0f;
        HullManager.Instance.health = (newHealth / 15.0f) * 100;
    }

    public void TakeDamage(float damage)
    {
        GameNetwork.Instance.ToPlayerQueue.Add("g:TechnicianTakeDamage:" + damage);
    }


    public void GameOver()
    {
        AudioManager.Instance.StopAll();
        Time.timeScale = 0.0f;
        GameOverPanel.SetActive(true);
    }

    public void SetScore(int scoreValue)
    {
        ScoreText.text = "SCORE: " + scoreValue;
    }

    public void ImageMatch_Help()
    {
        ImageMatchInstructionsObject.SetActive(true);
    }

    public void LockRotation_Help()
    {
        LockRotationInstructionsObject.SetActive(true);
    }

    public void TimingButton_Help()
    {
        TimingButtonInstructionsObject.SetActive(true);
    }

    public void RepairDamage(int newHealth)
    {
        HealthBar.Health = newHealth / 15.0f;
        AudioManager.Instance.PlaySound("Player_Repair");
    }
}
