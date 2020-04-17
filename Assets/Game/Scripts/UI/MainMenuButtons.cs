using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Photon.Realtime;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject FrontPageObject, MultiplayerPageObject, LobbyPageObject, SettingsPageObject;
    public GameObject InputDetectionPopupObject;
    public static MainMenuButtons Instance;

    public Text CreateRoomNameText, MultiplayerIssueLoggerText;
    public Text GunnerPlayerText, TechnicianPlayerText, PilotPlayerText;
    public GameObject AIDemoBtn, CreateGameBtn, LobbyBackBtn, SettingsBackBtn;
    public GameObject GunnerPlayerTextBG, TechnicianPlayerTextBG, PilotPlayerTextBG;
    public GameObject LobbyStartGameBtn;

    public GameObject RoomListingPrefab;
    public Transform RoomListingsParent;

    public PlayerLayoutGroup PlayerListings;
    public bool IsRoleSelected = false;
    public float TimeForIssueLogging = 4.0f;
    public Dictionary<string, string> PlayerRoleMapping = new Dictionary<string, string>() { { "Gunner", "" }, { "Pilot", "" }, { "Technician", "" } };

    private List<RoomListing> activeRoomListings = new List<RoomListing>();
    private List<RoomInfo> activeRoomInfo = new List<RoomInfo>();

    private RoomListing selectedRoomListing;
    private float currentTimeLogging = 0.0f;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        //DebugText.text = "# of Rooms: "  + PhotonNetwork.CountOfRooms + System.Environment.NewLine;
        //DebugText.text += "Currently in a lobby: " + PhotonNetwork.InLobby.ToString();

        if(currentTimeLogging > 0.0f)
        {
            currentTimeLogging += Time.deltaTime;
            if(currentTimeLogging > TimeForIssueLogging)
            {
                currentTimeLogging = 0.0f;
                MultiplayerIssueLoggerText.text = "";
            }
        }
        ClearEmptyRooms();
    }

    private void ClearEmptyRooms()
    {
        RoomListing rListingBehaviour;
        for (int i = activeRoomInfo.Count - 1; i >= 0; i--)
        {
            if (activeRoomInfo[i] == null ||
                activeRoomInfo[i].RemovedFromList ||
                !activeRoomInfo[i].IsVisible ||
                !activeRoomInfo[i].IsOpen ||
                activeRoomInfo[i].PlayerCount == 0)
            {
                if (activeRoomListings.Where(rl => rl.name == activeRoomInfo[i].Name).Count() == 1)
                {
                    rListingBehaviour = activeRoomListings.Single(rl => rl.name == activeRoomInfo[i].Name);
                    Destroy(rListingBehaviour.gameObject);
                    activeRoomListings.Remove(rListingBehaviour);
                    activeRoomInfo.Remove(activeRoomInfo[i]);
                }
            }
        }
    }

    public void InputTypeAccepted()
    {
        InputDetectionPopupObject.SetActive(false);
        PhotonNetwork.NickName = InputManager.Instance.inputMode.ToString() + "_" + Random.Range(0, 1000);
        FrontPageObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(AIDemoBtn);

    }

    #region FrontPage
    public void MainPage_AIDemoClick()
    {
        LobbyNetwork.Instance.LoadLevel("AI_Demo_Scene");
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
        EventSystem.current.SetSelectedGameObject(SettingsBackBtn);
    }

    public void MainPage_ExitClick()
    {
        //Exit the game
        Application.Quit();
    }
    #endregion

    #region Network Responses
    public void ConnectedToServer()
    {
        LobbyNetwork.Instance.JoinLobby();
    }

    public void JoinedLobby()
    {
        if (InputManager.Instance.inputMode == InputManager.InputMode.Null)
        {
            InputDetectionPopupObject.SetActive(true);
        }
    }

    public void JoinedRoom()
    {
        LobbyPageObject.SetActive(true);
        MultiplayerPageObject.SetActive(false);

        for(int i = activeRoomListings.Count - 1; i >= 0; i--)
        {
            Destroy(activeRoomListings[i].gameObject);
        }
        activeRoomListings.Clear();
        activeRoomInfo.Clear();
        EventSystem.current.SetSelectedGameObject(LobbyBackBtn);
    }

    public void FailedToCreateRoom(string message)
    {
        MultiplayerIssueLoggerText.text = "Could not create room - " + message;
        currentTimeLogging += Time.deltaTime;
    }

    public void LeftLobby()
    {
        FrontPageObject.SetActive(true);
    }

    public void LeftRoom()
    {
        MultiplayerPageObject.SetActive(true);
    }

    public void UpdateRoomListings(Dictionary<RoomInfo, bool> rooms)
    {
        GameObject newRoomListing;
        RoomListing rListingBehaviour;

        foreach (KeyValuePair<RoomInfo, bool> room in rooms)
        {
            if (room.Value)
            {
                if(!activeRoomListings.Any(rl => rl.RoomNameText.text == room.Key.Name))
                {
                    newRoomListing = Instantiate(RoomListingPrefab, RoomListingsParent);
                    rListingBehaviour = newRoomListing.GetComponent<RoomListing>();
                    if (rListingBehaviour)
                    {
                        rListingBehaviour.RoomNameText.text = room.Key.Name;
                        activeRoomListings.Add(rListingBehaviour);
                        activeRoomInfo.Add(room.Key);
                    }
                }

            }
            else
            {
                List<RoomListing> roomListingsToRemove = activeRoomListings.Where(rl => rl.RoomNameText.text == room.Key.Name).ToList();
                foreach(RoomListing roomListingToRemove in roomListingsToRemove)
                {
                    Destroy(roomListingToRemove.gameObject);
                    activeRoomListings.Remove(roomListingToRemove);
                }

            }
        }
    }

    #endregion

    #region Multiplayer Page
    public void MultiplayerPage_CreateGameOnClick()
    {
        if (!LobbyNetwork.Instance.CreateRoom(CreateRoomNameText.text))
        {
            MultiplayerIssueLoggerText.text = "Could not create room. Check if it already exists.";
            currentTimeLogging += Time.deltaTime;
        }
    }

    public void MultiplayerPage_JoinGameOnClick()
    {
        if(selectedRoomListing == null)
        {
            MultiplayerIssueLoggerText.text = "No room is currently selected.";
            currentTimeLogging += Time.deltaTime;
            return;
        }
        
        LobbyNetwork.Instance.JoinRoom(selectedRoomListing.RoomNameText.text);
    }

    public void MultiplayerPage_BackClick()
    {
        FrontPageObject.SetActive(true);
        MultiplayerPageObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(AIDemoBtn);
    }

    public void SelectRoomListing(RoomListing roomListing)
    {
        Debug.Log("Selected.");
        selectedRoomListing = roomListing;
    }
    #endregion

    #region Lobby Page
    public void LobbyPage_StartGameClick()
    {
        string loadingLevel = "";
        if (PhotonNetwork.IsMasterClient)
        {
            //Send load messages to other players
            if (GunnerPlayerText.text != "")
            {
                if (GunnerPlayerText.text == PhotonNetwork.NickName)
                {
                    loadingLevel = "Gunner";
                }
                else
                {
                    LobbyNetwork.Instance.BroadcastQueue.Add("TryPlayerLoadLevel:Gunner:" + GunnerPlayerText.text);
                }
            }

            if (PilotPlayerText.text != "")
            {
                if (PilotPlayerText.text == PhotonNetwork.NickName)
                {
                    loadingLevel = "Pilot";
                }
                else
                {
                    LobbyNetwork.Instance.BroadcastQueue.Add("TryPlayerLoadLevel:Pilot:" + PilotPlayerText.text);
                }
            }

            if (TechnicianPlayerText.text != "")
            {
                if (TechnicianPlayerText.text == PhotonNetwork.NickName)
                {
                    loadingLevel = "Technician";
                }
                else
                {
                    LobbyNetwork.Instance.BroadcastQueue.Add("TryPlayerLoadLevel:Technician:" + TechnicianPlayerText.text);
                }
            }

            if (loadingLevel != "")
            {
                LobbyNetwork.Instance.LoadLevel(loadingLevel);
            }
        }
    }

    public void LobbyPage_GunnerClick()
    {
        if (GunnerPlayerText.text == "" && !IsRoleSelected)
        {
            TrySelectingRole("Gunner", PhotonNetwork.NickName);
        }
        else if (GunnerPlayerText.text == PhotonNetwork.LocalPlayer.NickName)
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

        foreach (KeyValuePair<string, string> role in PlayerRoleMapping)
        {
            if (role.Value == playerName)
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
            LobbyNetwork.Instance.BroadcastQueue.Add("SelectPlayerRole:" + roleName + ":" + playerName);
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
            if (roleText.text == playerName)
            {
                roleText.text = "";
                roleBG.SetActive(false);
                if (playerName == PhotonNetwork.NickName)
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
            if (GunnerPlayerText.text == PhotonNetwork.NickName)
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
        PlayerRoleMapping["Gunner"] = "";
        PlayerRoleMapping["Pilot"] = "";
        PlayerRoleMapping["Technician"] = "";
        GunnerPlayerText.text = "";
        PilotPlayerText.text = "";
        TechnicianPlayerText.text = "";
        IsRoleSelected = false;
        LobbyNetwork.Instance.SendEvents();
        LobbyPageObject.SetActive(false);
        LobbyNetwork.Instance.LeaveRoom();

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
        if (gunnerName == "")
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
    #endregion

    #region Settings Page
    public void SettingsPage_VolumeChanged(float newVolume)
    {
        AudioManager.Instance.SetGameVolume(newVolume);
    }

    public void SettingsPage_BackClick()
    {
        SettingsPageObject.SetActive(false);
        FrontPageObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(AIDemoBtn);
    }
    #endregion
}
