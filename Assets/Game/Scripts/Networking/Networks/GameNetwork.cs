using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Text DebugText;

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
            DebugText.text += queuedEvent + System.Environment.NewLine;
            newEventMessage = queuedEvent;
            PhotonNetwork.RaiseEvent(GAME_BROADCAST_EVENT, newEventMessage, RaiseEventOptions.Default, SendOptions.SendReliable);
        }

        foreach (string queuedEvent in ToPlayerQueue)
        {
            DebugText.text += queuedEvent + System.Environment.NewLine;
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
                    case "TechnicianMessengerReset":
                        TechnicianMessenger.Instance.ResetMessenger();
                        break;
                    default:
                        break;
                }

                break;
            case GAME_TOPLAYER_EVENT:
                datas = obj.CustomData;
                messageSegments = datas.ToString().Split(':');
                DebugText.text += messageSegments[0] + messageSegments[1] + System.Environment.NewLine;
                if (getPlayerTypeFromCode(messageSegments[0]) == Type)
                {
                    switch (messageSegments[1])
                    {
                        case "MiniGameIMChooseIcon":
                            TechnicianMessenger.Instance.UpdateIcon(messageSegments);
                            break;
                        case "MiniGameLRSetLockInvalids":
                            TechnicianMessenger.Instance.UpdateLockMessage(messageSegments[2]);
                            break;
                        case "MiniGameTBChoosePatterns":
                            TechnicianMessenger.Instance.UpdatePatterns(messageSegments[3], messageSegments[5], messageSegments[7]);
                            break;
                        case "MiniGameTBSetColours":
                            TechnicianMessenger.Instance.UpdateColours(messageSegments);
                            break;
                        case "PilotTransformUpdate":
                            GunnerController.Instance.UpdatePlayer(messageSegments);
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
