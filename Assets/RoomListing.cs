using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    public Text RoomNameText;

    public void Select()
    {
        MainMenuButtons.Instance.SelectRoomListing(this);
    }
}
