﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject FrontPageObject, MultiplayerPageObject, LobbyPageObject, SettingsPageObject;
    public GameObject ChooseNamePopupObject, CreateGamePopupObject, JoinGamePopupObject, InputDetectionPopupObject;
    public static MainMenuButtons Instance;
    public Text CreateRoomName, JoinRoomName, NameEnterText;
    public Text GunnerPlayerText, TechnicianPlayerText, PilotPlayerText;
    public GameObject GunnerPlayerTextBG, TechnicianPlayerTextBG, PilotPlayerTextBG;
    public GameObject LobbyStartGameBtn;
    public LoginInformation CreateGameLoginInfo, JoinGameLoginInfo;
    public PlayerLayoutGroup PlayerListings;
    public bool IsRoleSelected = false;
    private bool hasName = false;
    public Dictionary<string, string> PlayerRoleMapping = new Dictionary<string, string>() { { "Gunner", "" }, { "Pilot", "" }, { "Technician", "" } };
    public Text DebugText;

    public GameObject AIDemoBtn, CreateGameBtn, LobbyBackBtn;
    private void Start()
    {
        hasName = false;
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        //DebugText.text = IsRoleSelected.ToString();
    }

    public void InputTypeAccepted()
    {
        InputDetectionPopupObject.SetActive(false);
        DebugText.text = InputManager.Instance.inputMode.ToString();
        PhotonNetwork.NickName = InputManager.Instance.inputMode.ToString() + "_" + Random.Range(0, 1000);
        FrontPageObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(AIDemoBtn);
        //if (!hasName)
        //{
        //    string temp = PlayerPrefs.GetString("PlayerName");
        //    if (temp != "")
        //    {
        //        hasName = true;
        //        PhotonNetwork.NickName = temp + "_" + Random.Range(0, 1000);
        //        FrontPageObject.SetActive(true);
        //    }
        //    else
        //    {
        //        ChooseNamePopupObject.SetActive(true);
        //    }
        //}
    }

    public void MainPage_AIDemoClick()
    {
        LobbyNetwork.Instance.LoadLevel("AI_Demo_Scene");
    }

    public void ConnectedToServer()
    {
        LobbyNetwork.Instance.JoinLobby();
    }

    public void MainPage_MultiplayerOnClick()
    {
        FrontPageObject.SetActive(false);
        MultiplayerPageObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(CreateGameBtn);
    }

    public void MainPage_SettingsClick()
    {
        SettingsPageObject.SetActive(true);
        FrontPageObject.SetActive(false);
    }

    public void MainPage_ExitClick()
    {
        //Exit the game
        Application.Quit();
    }

    public void JoinedLobby()
    {
        if(InputManager.Instance.inputMode == InputManager.InputMode.Null)
        {
            InputDetectionPopupObject.SetActive(true);
        }
    }

    public void JoinedRoom()
    {
        LobbyPageObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(LobbyBackBtn);
        JoinGamePopupObject.SetActive(false);
        MultiplayerPageObject.SetActive(false);
    }

    public void MultiplayerPage_CreateGameOnClick()
    {
        //CreateGamePopupObject.SetActive(true);
        LobbyNetwork.Instance.CreateRoom("TestRoom");
        CreateGamePopupObject.SetActive(false);
        MultiplayerPageObject.SetActive(false);
    }

    public void MultiplayerPage_JoinGameOnClick()
    {
        LobbyNetwork.Instance.JoinRoom("TestRoom");
        //JoinGamePopupObject.SetActive(true);
    }

    public void MultiplayerPage_BackClick()
    {
        FrontPageObject.SetActive(true);
        MultiplayerPageObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(AIDemoBtn);

    }

    public void CreateGamePopup_CreateClick()
    {

        if (LobbyNetwork.Instance.CreateRoom(CreateRoomName.text))
        {
            CreateGamePopupObject.SetActive(false);
            MultiplayerPageObject.SetActive(false);
        }
        else
        {
            //display error message
        }
    }

    public void CreateGamePopup_CancelClick()
    {
        CreateGamePopupObject.SetActive(false);
    }

    public void JoinGamePopup_JoinClick()
    {
        LobbyNetwork.Instance.JoinRoom(JoinRoomName.text);
    }

    public void JoinGamePopup_CancelClick()
    {
        JoinGamePopupObject.SetActive(false);
    }

    public void LobbyPage_StartGameClick()
    {
        string loadingLevel = "";
        if (PhotonNetwork.IsMasterClient)
        {
            //Send load messages to other players
            if(GunnerPlayerText.text != "")
            {
                if(GunnerPlayerText.text == PhotonNetwork.NickName)
                {
                    loadingLevel = "Gunner_Placeholder";
                }
                else
                {
                    LobbyNetwork.Instance.BroadcastQueue.Add("TryPlayerLoadLevel:Gunner_Placeholder:" + GunnerPlayerText.text);
                }
            }

            if (PilotPlayerText.text != "")
            {
                if (PilotPlayerText.text == PhotonNetwork.NickName)
                {
                    loadingLevel = "Pilot_Placeholder";
                }
                else
                {
                    LobbyNetwork.Instance.BroadcastQueue.Add("TryPlayerLoadLevel:Pilot_Placeholder:" + PilotPlayerText.text);
                }
            }

            if (TechnicianPlayerText.text != "")
            {
                if (TechnicianPlayerText.text == PhotonNetwork.NickName)
                {
                    loadingLevel = "Technician_Placeholder";
                }
                else
                {
                    LobbyNetwork.Instance.BroadcastQueue.Add("TryPlayerLoadLevel:Technician_Placeholder:" + TechnicianPlayerText.text);
                }
            }

            if(loadingLevel != "")
            {
                LobbyNetwork.Instance.LoadLevel(loadingLevel);
            }
        }
    }

    public void AcceptName(string name)
    {
        if(PhotonNetwork.NickName == name)
        {
            ChooseNamePopupObject.SetActive(false);
            if(!SettingsPageObject.activeSelf && !FrontPageObject.activeSelf)
            {
                FrontPageObject.SetActive(true);
            }
            hasName = true;
            PlayerPrefs.SetString("PlayerName", name.Split('_')[0]);
        }
    }

    public void LobbyPage_GunnerClick()
    {
        if(GunnerPlayerText.text == "" && !IsRoleSelected)
        {
            TrySelectingRole("Gunner", PhotonNetwork.NickName);
        }
        else if(GunnerPlayerText.text == PhotonNetwork.LocalPlayer.NickName)
        {
            TryDeselectingRole("Gunner", PhotonNetwork.NickName);
        }
    }

    public void LobbyPage_PilotClick()
    {
        if (PilotPlayerText.text == "" && !IsRoleSelected)
        {
            TrySelectingRole("Pilot", PhotonNetwork.NickName);
        }
        else if (PilotPlayerText.text == PhotonNetwork.LocalPlayer.NickName)
        {
            TryDeselectingRole("Pilot", PhotonNetwork.NickName);
        }
    }

    public void LobbyPage_TechnicianClick()
    {
        if (TechnicianPlayerText.text == "" && !IsRoleSelected)
        {
            TrySelectingRole("Technician", PhotonNetwork.NickName);
        }
        else if (TechnicianPlayerText.text == PhotonNetwork.LocalPlayer.NickName)
        {
            TryDeselectingRole("Technician", PhotonNetwork.NickName);
        }
    }

    public bool TrySelectingRole(string roleName, string playerName)
    {
        Text roleText;
        GameObject roleBG;

        switch (roleName)
        {
            case "Gunner":
                roleText = GunnerPlayerText;
                roleBG = GunnerPlayerTextBG;
                break;
            case "Pilot":
                roleText = PilotPlayerText;
                roleBG = PilotPlayerTextBG;
                break;
            case "Technician":
                roleText = TechnicianPlayerText;
                roleBG = TechnicianPlayerTextBG;
                break;
            default:
                return false;
        }

        foreach(KeyValuePair<string,string> role in PlayerRoleMapping)
        {
            if(role.Value == playerName)
            {
                return false;
            }
        }


        if (PhotonNetwork.IsMasterClient)
        {
            roleText.text = playerName;
            if (playerName == PhotonNetwork.NickName)
            {
                IsRoleSelected = true;
            }
            LobbyNetwork.Instance.BroadcastQueue.Add("SelectPlayerRole:"+ roleName + ":" + playerName);
            SelectPlayerRole(roleName, playerName);
            PlayerRoleMapping[roleName] = playerName;
        }
        else
        {
            LobbyNetwork.Instance.FromClientQueue.Add("TrySelectPlayerRole:" + roleName);
        }
        return true;
    }

    public bool TryDeselectingRole(string roleName, string playerName)
    {
        Text roleText;
        GameObject roleBG;

        switch (roleName)
        {
            case "Gunner":
                roleText = GunnerPlayerText;
                roleBG = GunnerPlayerTextBG;
                break;
            case "Pilot":
                roleText = PilotPlayerText;
                roleBG = PilotPlayerTextBG;
                break;
            case "Technician":
                roleText = TechnicianPlayerText;
                roleBG = TechnicianPlayerTextBG;
                break;
            default:
                return false;
        }

        bool foundMatchingPlayer = false;
        foreach (KeyValuePair<string, string> role in PlayerRoleMapping)
        {
            if (role.Value == playerName)
            {
                foundMatchingPlayer = true;
            }
        }

        if (!foundMatchingPlayer)
        {
            return false;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if(roleText.text == playerName)
            {
                roleText.text = "";
                roleBG.SetActive(false);
                if(playerName == PhotonNetwork.NickName)
                {
                    IsRoleSelected = false;
                }
                PlayerRoleMapping[roleName] = "";
                LobbyNetwork.Instance.BroadcastQueue.Add("DeselectPlayerRole:" + roleName);
                DeselectPlayerRole(roleName);
            }
        }
        else
        {
            LobbyNetwork.Instance.FromClientQueue.Add("TryDeselectPlayerRole:" + roleName);
        }

        return true;
    }

    public void LobbyPage_DisconnectClick()
    {
        if (IsRoleSelected)
        {
            //IsRoleSelected = false;
            if(GunnerPlayerText.text == PhotonNetwork.NickName)
            {
                GunnerPlayerText.text = "";
                GunnerPlayerTextBG.SetActive(false);
                LobbyNetwork.Instance.BroadcastQueue.Add("DeselectPlayerRole:Gunner");
            }
            else if (TechnicianPlayerText.text == PhotonNetwork.NickName)
            {
                TechnicianPlayerText.text = "";
                TechnicianPlayerTextBG.SetActive(false);
                LobbyNetwork.Instance.BroadcastQueue.Add("DeselectPlayerRole:Technician");
            }
            else if (PilotPlayerText.text == PhotonNetwork.NickName)
            {
                PilotPlayerText.text = "";
                PilotPlayerTextBG.SetActive(false);
                LobbyNetwork.Instance.BroadcastQueue.Add("DeselectPlayerRole:Pilot");
            }
        }
        IsRoleSelected = false;
        LobbyNetwork.Instance.SendEvents();
        LobbyPageObject.SetActive(false);
        LobbyNetwork.Instance.LeaveRoom();
        EventSystem.current.SetSelectedGameObject(CreateGameBtn);
    }

    public void SettingsPage_VolumeChanged(float newVolume)
    {
        AudioManager.Instance.SetGameVolume(newVolume);
    }

    public void SettingsPage_ChangeNameClick()
    {
        ChooseNamePopupObject.SetActive(true);
    }

    public void SettingsPage_BackClick()
    {
        SettingsPageObject.SetActive(false);
        FrontPageObject.SetActive(true);
    }

    public void LeftLobby()
    {
        FrontPageObject.SetActive(true);
    }

    public void LeftRoom()
    {
        MultiplayerPageObject.SetActive(true);
    }

    public void NameEnter_OKClick()
    {
        LobbyNetwork.Instance.ChooseName(NameEnterText.text);
    }

    public void SelectPlayerRole(string roleType, string playerName)
    {
        switch (roleType)
        {
            case "Gunner":
                GunnerPlayerText.text = playerName;
                GunnerPlayerTextBG.SetActive(true);
                break;
            case "Pilot":
                PilotPlayerText.text = playerName;
                PilotPlayerTextBG.SetActive(true);
                break;
            case "Technician":
                TechnicianPlayerText.text = playerName;
                TechnicianPlayerTextBG.SetActive(true);
                break;
            default:
                return;
        }
        PlayerRoleMapping[roleType] = playerName;
    }

    public void DeselectPlayerRole(string roleType)
    {
        switch (roleType)
        {
            case "Gunner":
                GunnerPlayerText.text = "";
                GunnerPlayerTextBG.SetActive(false);
                break;
            case "Pilot":
                PilotPlayerText.text = "";
                PilotPlayerTextBG.SetActive(false);
                break;
            case "Technician":
                TechnicianPlayerText.text = "";
                TechnicianPlayerTextBG.SetActive(false);
                break;
            default:
                return;
        }
        PlayerRoleMapping[roleType] = "";
    }

    public void UpdatePlayerRoleCircles(string gunnerName, string technicianName, string pilotName)
    {
        if(gunnerName == "")
        {
            DeselectPlayerRole(gunnerName);
        }
        else
        {
            SelectPlayerRole("Gunner", gunnerName);
        }

        if (technicianName == "")
        {
            DeselectPlayerRole(technicianName);
        }
        else
        {
            SelectPlayerRole("Technician", technicianName);
        }

        if (pilotName == "")
        {
            DeselectPlayerRole(pilotName);
        }
        else
        {
            SelectPlayerRole("Pilot", pilotName);
        }
    }

}
