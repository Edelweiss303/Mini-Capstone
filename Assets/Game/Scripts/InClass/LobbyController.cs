using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject m_playNowButton;

    [SerializeField]
    private GameObject m_cancelButton;



    // Start is called before the first frame update
    void Start()
    {
        m_playNowButton.SetActive(false);
        m_cancelButton.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        m_playNowButton.SetActive(true);
    }

    public void PlayNow()
    {
        m_playNowButton.SetActive(false);
        m_cancelButton.SetActive(true);
        Debug.Log("Play Now Clicked!");
    }
    
    public void Cancel()
    {
        m_cancelButton.SetActive(false);
        m_playNowButton.SetActive(true);
        Debug.Log("Cancel Clicked!");
    }
}
