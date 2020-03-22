using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LobbyCanvas : MonoBehaviour
{
    [SerializeField]
    private RoomLayoutGroup _roomLayoutGroup;

    private RoomLayoutGroup RoomLayoutGroup
    {
        get { return _roomLayoutGroup; }
    }
    
    public void OnClick_JoinRoom(string roomName)
    {
        if (!PhotonNetwork.JoinRoom(roomName))
        {
            print("Join room failed.");
        }
    }
}
