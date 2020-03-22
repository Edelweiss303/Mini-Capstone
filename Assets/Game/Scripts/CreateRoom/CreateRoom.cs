using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _roomName;
    private Text RoomName
    {
        get { return _roomName; }
    }

    public Text ErrorText;

    public void OnClick_CreateRoom()
    {
        if(RoomName.text == "")
        {
            ErrorText.text = "Please enter a room name.";
        }
        if (!LobbyNetwork.Instance.CreateRoom(RoomName.text))
        {
            ErrorText.text = RoomName.text + " is already being used as a room name.";
        }
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("Create room failed: " + codeAndMessage[1]);
    }

    override public void OnCreatedRoom()
    {
        print("Room created successfully");

    }
}
