using Photon.Pun;
using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour
{
    public void OnClick_StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.LoadLevel(1);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(1);
        }
        
    }

    public void OnClick_StartDelayed()
    {

    }
}
