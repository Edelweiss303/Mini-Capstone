using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;

using System.Collections;

using PHash = ExitGames.Client.Photon;

[RequireComponent(typeof(PhotonView))]
public class PlayerNetwork : MonoBehaviourPunCallbacks
{
    public static PlayerNetwork Instance;
    public string PlayerName { get; private set; }
    private PhotonView PhotonView;
    private int PlayersInGame = 0;
    private PHash.Hashtable m_playerCustomProperties = new PHash.Hashtable();
    private Coroutine m_pingCoroutine;

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        PlayerName = PhotonNetwork.NickName;

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
        PhotonView = GetComponent<PhotonView>();
        PhotonView.ViewID = 998;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Game")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MasterLoadedGame();
            }
            else
            {
                NonMasterLoadedGame();
            }
        }
    }

    private void MasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        PhotonView.RPC("RPC_LoadGameOthers", RpcTarget.Others);
    }

    private void NonMasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(1);
    }

    [PunRPC]
    private void RPC_LoadedGameScene(Player inPhotonPlayer)
    {
        PlayersInGame++;

        if(PlayersInGame == PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("All players are in the game.");
        }
    }

    private IEnumerator C_SetPing()
    {
        while (PhotonNetwork.IsConnected)
        {
            m_playerCustomProperties["Ping"] = PhotonNetwork.GetPing();
            PhotonNetwork.LocalPlayer.SetCustomProperties(m_playerCustomProperties);

            yield return new WaitForSeconds(5.0f);
        }

        yield break;
    }

    public override void OnConnectedToMaster()
    {
        if (m_pingCoroutine != null)
        {
            StopCoroutine(m_pingCoroutine);
        }
        m_pingCoroutine = StartCoroutine(C_SetPing());

    }
}
