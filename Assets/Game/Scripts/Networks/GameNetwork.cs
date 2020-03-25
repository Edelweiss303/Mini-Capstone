using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetwork : MonoBehaviour
{
    public enum PlayerType
    {
        Gunner, Pilot, Technician
    }
    public PlayerType Type;
    public static GameNetwork Instance;

    private const byte GAME_BROADCAST_EVENT = 4;
    private const byte GAME_TOPLAYER_EVENT = 5;

    public List<string> ToPlayerQueue = new List<string>();
    public List<string> BroadcastQueue = new List<string>();

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    void Update()
    {
        SendEvents();
    }

    public void SendEvents()
    {
        object newEventMessage;

        foreach (string queuedEvent in BroadcastQueue)
        {
            Debug.Log("Sending message: " + queuedEvent);
            newEventMessage = queuedEvent;
            PhotonNetwork.RaiseEvent(GAME_BROADCAST_EVENT, newEventMessage, RaiseEventOptions.Default, SendOptions.SendReliable);
        }

        foreach (string queuedEvent in ToPlayerQueue)
        {
            Debug.Log("Sending message: " + queuedEvent);
            newEventMessage = queuedEvent;
            PhotonNetwork.RaiseEvent(GAME_TOPLAYER_EVENT, newEventMessage, RaiseEventOptions.Default, SendOptions.SendReliable);
        }

        BroadcastQueue.Clear();
        ToPlayerQueue.Clear();
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {
        object datas;
        string[] messageSegments;
        switch (obj.Code)
        {
            case GAME_BROADCAST_EVENT:
                datas = obj.CustomData;
                messageSegments = datas.ToString().Split(':');
                switch (messageSegments[0])
                {
                    default:
                        break;
                }

                break;
            case GAME_TOPLAYER_EVENT:
                datas = obj.CustomData;
                messageSegments = datas.ToString().Split(':');
                Debug.Log("Receiving message: " + messageSegments[0] + messageSegments[1]);
                if (getPlayerTypeFromCode(messageSegments[0]) == Type)
                {
                    switch (messageSegments[1])
                    {
                        case "ChooseIcon":
                            TechnicianMessenger.Instance.UpdateIcon(messageSegments);
                            break;
                        default:
                            break;
                    }
                }
                break;
        }
    }

    private string getCodeFromPlayerType(PlayerType inType)
    {
        switch (inType)
        {
            case PlayerType.Gunner:
                return "g";
            case PlayerType.Pilot:
                return "p";
            case PlayerType.Technician:
                return "t";
        }
        return "";
    }

    private PlayerType getPlayerTypeFromCode(string inCode)
    {
        switch (inCode)
        {
            case "g":
                return PlayerType.Gunner;
            case "p":
                return PlayerType.Pilot;
            case "t":
                return PlayerType.Technician;
        }

        return PlayerType.Gunner;
    }
}
