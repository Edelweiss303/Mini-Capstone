using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyNetwork : MonoBehaviourPunCallbacks
{
    public static LobbyNetwork Instance;

    private const byte LOBBY_BROADCAST_EVENT = 1;
    private const byte LOBBY_FROMCLIENT_EVENT = 2;
    private const byte LOBBY_TOCLIENT_EVENT = 3;

    public List<string> FromClientQueue = new List<string>();
    public List<string> ToClientQueue = new List<string>();
    public List<string> BroadcastQueue = new List<string>();

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.fullScreen = false;
        if (Instance == null)
        {
            Instance = this;
        }

        if (!PhotonNetwork.IsConnected)
        {
            print("Connecting to server..");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Update()
    {
        SendEvents();
    }

    #region Create/Join/Connect Callbacks
    public override void OnConnectedToMaster()
    {
        print("Connected to the server.");
        MainMenuButtons.Instance.ConnectedToServer();
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    public override void OnCreatedRoom()
    {
        print("Created room." + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedLobby()
    {
        if (!PhotonNetwork.InRoom)
        {
            print("Joined lobby.");
            MainMenuButtons.Instance.JoinedLobby();
        }
    }

    public void JoinLobby()
    {
        if (!PhotonNetwork.JoinLobby(TypedLobby.Default))
        {
            Debug.Log("Failed to join lobby.");
        }

    }

    public void JoinRoom(string roomName)
    {
        Debug.Log("Trying to join room.");
        if (!PhotonNetwork.JoinRoom(roomName))
        {
            Debug.Log("Failed to join room.");
        }
    }


    public override void OnJoinedRoom()
    {
        print("Joined room.");
        MainMenuButtons.Instance.JoinedRoom();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Player[] photonPlayers = PhotonNetwork.PlayerList;
        Debug.Log(photonPlayers.Length);
        foreach (Player photonPlayer in photonPlayers)
        {
            MainMenuButtons.Instance.PlayerListings.PlayerJoinedRoom(photonPlayer);
        }

        MainMenuButtons.Instance.LobbyStartGameBtn.SetActive(PhotonNetwork.IsMasterClient);

    }

    public bool CreateRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 3 };

        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        Debug.Log("Creating room.");
        return true;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MainMenuButtons.Instance.FailedToCreateRoom(message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        MainMenuButtons.Instance.PlayerListings.PlayerJoinedRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient)
        {
            //Update the circles
            LobbyNetwork.Instance.BroadcastQueue.Add("PlayerJoinUpdateCircles:" +
                                                        "G:" + MainMenuButtons.Instance.PlayerRoleMapping["Gunner"] + ":" +
                                                        "T:" + MainMenuButtons.Instance.PlayerRoleMapping["Technician"] + ":" +
                                                        "P:" + MainMenuButtons.Instance.PlayerRoleMapping["Pilot"]);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Dictionary<RoomInfo, bool> rooms = new Dictionary<RoomInfo, bool>();

       // string test = "";
        foreach(RoomInfo room in roomList)
        {
            rooms.Add(room,!room.RemovedFromList && room.IsOpen && room.IsVisible);
            //test += room.Name + "-" + (room.RemovedFromList ? "R" : "E") + "-" + (room.IsOpen ? "O" : "C") + "-" + (room.IsVisible ? "V" : "I") + ", ";

        }
       // MainMenuButtons.Instance.MultiplayerIssueLoggerText.text = test;
        MainMenuButtons.Instance.UpdateRoomListings(rooms);
    }
    #endregion

    #region Leave Callbacks
    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
        MainMenuButtons.Instance.LeftLobby();
    }

    public void LeaveRoom()
    {
        
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        MainMenuButtons.Instance.LeftRoom();
        print("Left room.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from room.");
       // MainMenuButtons.Instance.LobbyPage_DisconnectClick();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        MainMenuButtons.Instance.PlayerListings.PlayerLeftRoom(otherPlayer);
    }
    #endregion

    #region Failure Callbacks
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        MainMenuButtons.Instance.DebugText.text = "Failed to join room: " + returnCode + "-" + message;
        Debug.Log("Failed to join room: " + returnCode + "-" + message);
    }
    #endregion

    #region EventHandling
    public void SendEvents()
    {
        object newEventMessage;

        foreach (string queuedEvent in BroadcastQueue)
        {
            newEventMessage = queuedEvent;
            PhotonNetwork.RaiseEvent(LOBBY_BROADCAST_EVENT, newEventMessage, RaiseEventOptions.Default, SendOptions.SendReliable);
        }

        foreach (string queuedEvent in FromClientQueue)
        {
            Debug.Log(queuedEvent);
            newEventMessage = PhotonNetwork.NickName + ":" + queuedEvent;
            PhotonNetwork.RaiseEvent(LOBBY_FROMCLIENT_EVENT, newEventMessage, RaiseEventOptions.Default, SendOptions.SendReliable);
        }

        foreach (string queuedEvent in ToClientQueue)
        {
            newEventMessage = queuedEvent;
            PhotonNetwork.RaiseEvent(LOBBY_TOCLIENT_EVENT, newEventMessage, RaiseEventOptions.Default, SendOptions.SendReliable);
        }

        BroadcastQueue.Clear();
        FromClientQueue.Clear();
        ToClientQueue.Clear();
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {
        object datas;
        string[] messageSegments;
        switch (obj.Code)
        {
            case LOBBY_BROADCAST_EVENT:
                datas = obj.CustomData;
                messageSegments = datas.ToString().Split(':');
                switch (messageSegments[0])
                {
                    case ("SelectPlayerRole"):
                        MainMenuButtons.Instance.SelectPlayerRole(messageSegments[1], messageSegments[2]);
                        break;
                    case ("DeselectPlayerRole"):
                        MainMenuButtons.Instance.DeselectPlayerRole(messageSegments[1]);
                        break;
                    case ("PlayerJoinUpdateCircles"):
                        MainMenuButtons.Instance.UpdatePlayerRoleCircles(messageSegments[2], messageSegments[4], messageSegments[6]);
                        break;
                    case ("TryPlayerLoadLevel"):
                        if (messageSegments[2] == PhotonNetwork.NickName)
                        {
                            LoadLevel(messageSegments[1]);
                        }
                        break;

                }

                break;
            case LOBBY_FROMCLIENT_EVENT:
                datas = obj.CustomData;
                messageSegments = datas.ToString().Split(':');
                if (PhotonNetwork.IsMasterClient)
                {
                    switch (messageSegments[1])
                    {
                        case ("TrySelectPlayerRole"):

                            if (MainMenuButtons.Instance.TrySelectingRole(messageSegments[2], messageSegments[0]))
                            {
                                ToClientQueue.Add(messageSegments[0] + ":SuccessfulSelect:" + messageSegments[2]);
                            }
                            break;
                        case ("TryDeselectPlayerRole"):
                            if (MainMenuButtons.Instance.TryDeselectingRole(messageSegments[2], messageSegments[0]))
                            {
                                ToClientQueue.Add(messageSegments[0] + ":SuccessfulDeselect:" + messageSegments[2]);
                            }
                            break;
                    }
                }
                break;
            case LOBBY_TOCLIENT_EVENT:
                datas = obj.CustomData;
                messageSegments = datas.ToString().Split(':');
                if (messageSegments[0] == PhotonNetwork.NickName)
                {
                    switch (messageSegments[1])
                    {
                        case ("SuccessfulSelect"):
                            MainMenuButtons.Instance.IsRoleSelected = true;
                            MainMenuButtons.Instance.PlayerRoleMapping[messageSegments[2]] = PhotonNetwork.NickName;
                            break;
                        case ("SuccessfulDeselect"):
                            MainMenuButtons.Instance.IsRoleSelected = false;
                            MainMenuButtons.Instance.PlayerRoleMapping[messageSegments[2]] = "";
                            break;
                    }
                }

                break;
        }
    }
    #endregion

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }
}
