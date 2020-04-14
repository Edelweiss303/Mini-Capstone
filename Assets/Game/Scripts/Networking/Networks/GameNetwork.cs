using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private const byte GAME_ENEMYUPDATE_EVENT = 6;
    private const byte GAME_PROJECTILEUPDATE_EVENT = 7;

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
        if (PhotonNetwork.InRoom)
        {
            object newEventMessage;

            foreach (string queuedEvent in BroadcastQueue)
            {
                newEventMessage = queuedEvent;
                PhotonNetwork.RaiseEvent(GAME_BROADCAST_EVENT, newEventMessage, RaiseEventOptions.Default, SendOptions.SendReliable);
            }

            foreach (string queuedEvent in ToPlayerQueue)
            {
                newEventMessage = queuedEvent;
                PhotonNetwork.RaiseEvent(GAME_TOPLAYER_EVENT, newEventMessage, RaiseEventOptions.Default, SendOptions.SendReliable);
            }

            BroadcastQueue.Clear();
            ToPlayerQueue.Clear();
        }

    }

    public void UpdateEnemies(string message)
    {
        if (PhotonNetwork.InRoom)
        {
            object newMessage = message;
            PhotonNetwork.RaiseEvent(GAME_ENEMYUPDATE_EVENT, newMessage, RaiseEventOptions.Default, SendOptions.SendReliable);
        }

    }

    public void UpdateProjectiles(string message)
    {
        if (PhotonNetwork.InRoom)
        {
            object newMessage = message;
            PhotonNetwork.RaiseEvent(GAME_PROJECTILEUPDATE_EVENT, newMessage, RaiseEventOptions.Default, SendOptions.SendReliable);
        }
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
                    case "TechnicianHeat":
                        if(Type != PlayerType.Technician)
                        {
                            TechnicianMessenger.Instance.SetHeat(messageSegments);
                        }
                        break;
                    case "TechnicianOverheated":
                        if(Type != PlayerType.Technician)
                        {
                            TechnicianMessenger.Instance.SetOverheated();
                        }
                        break;
                    case "GunnerTakeDamage":
                        if(Type == PlayerType.Pilot)
                        {
                            PilotController.Instance.SetHealth(float.Parse(messageSegments[1]));
                            PilotController.Instance.PlayerViewScreen.SetDamageScreen();
                        }
                        else if(Type == PlayerType.Technician)
                        {
                            TechnicianController.Instance.SetHealth(float.Parse(messageSegments[1]));
                            TechnicianController.Instance.PlayerViewScreen.SetDamageScreen();
                        }
                        break;
                    case "GunnerGameOver":
                        if (Type == PlayerType.Pilot)
                        {
                            PilotController.Instance.GameOver();
                        }
                        else if (Type == PlayerType.Technician)
                        {
                            TechnicianController.Instance.GameOver();
                        }
                        break;
                    case "GunnerSetScore":
                        if(Type == PlayerType.Pilot)
                        {
                            PilotController.Instance.SetScore(int.Parse(messageSegments[1]));
                            
                        }
                        else if(Type == PlayerType.Technician)
                        {
                            TechnicianController.Instance.SetScore(int.Parse(messageSegments[1]));
                        }
                        break;
                    default:
                        break;
                }

                break;
            case GAME_TOPLAYER_EVENT:
                datas = obj.CustomData;
                messageSegments = datas.ToString().Split(':');
                if (getPlayerTypeFromCode(messageSegments[0]) == Type)
                {
                    switch (messageSegments[1])
                    {
                        case "MiniGameIMChooseIcon":
                            TechnicianMessenger.Instance.ResetMessenger();
                            TechnicianMessenger.Instance.UpdateIcon(messageSegments);
                            break;
                        case "MiniGameLRSetLockInvalids":
                            TechnicianMessenger.Instance.ResetMessenger();
                            TechnicianMessenger.Instance.UpdateLockMessage(messageSegments[2]);
                            break;
                        case "MiniGameTBChoosePatterns":
                            TechnicianMessenger.Instance.ResetMessenger();
                            TechnicianMessenger.Instance.UpdatePatterns(messageSegments[2]);
                            break;
                        case "MiniGameTBSetColours":
                            TechnicianMessenger.Instance.ResetMessenger();
                            TechnicianMessenger.Instance.UpdateColours(messageSegments);
                            break;
                        case "PilotTransformUpdate":
                            GunnerController.Instance.UpdatePlayer(messageSegments);
                            break;
                        case "GunnerGetAmmo":
                            GunnerController.Instance.UpdateAmmo(messageSegments);
                            break;
                        case "GunnerCreateAmmo":
                            GunnerController.Instance.AddAmmoPickup(messageSegments);
                            break;
                        case "TechAddHeat":
                            TechnicianController.Instance.IncreaseHeat(float.Parse(messageSegments[2]));
                            break;
                        case "GunnerSetPowerup":
                            GunnerController.Instance.SetPowerup(messageSegments);
                            break;
                        case "PilotRadarScan":
                            PilotController.Instance.ScanRadar();
                            break;
                        case "TechnicianTakeDamage":
                            GunnerController.Instance.PlayerObject.GetComponent<PlayerBehaviour>().TakeDamage(float.Parse(messageSegments[2]));
                            break;
                        default:
                            break;
                    }
                }
                break;
            case GAME_ENEMYUPDATE_EVENT:
                if(Type == PlayerType.Pilot)
                {
                    datas = obj.CustomData;
                    EnemiesManager.Instance.UpdateEnemies(datas.ToString());
                }

                break;
            case GAME_PROJECTILEUPDATE_EVENT:
                if (Type == PlayerType.Pilot)
                {
                    datas = obj.CustomData;
                    ProjectilesManager.Instance.UpdateProjectiles(datas.ToString());
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

    public void GameOver_ReturnToLobby()
    {
        Application.Quit();
        //PhotonNetwork.Disconnect();
        //SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
