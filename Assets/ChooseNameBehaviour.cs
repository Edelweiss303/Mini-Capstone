using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseNameBehaviour : MonoBehaviour
{
    public Text ChosenName;
    public Text ErrorText;

    public void OnClick_ChooseName()
    {
        if(ChosenName.text == "")
        {
            ErrorText.text = "Please enter a name before continuing.";
        }
        else if (LobbyNetwork.Instance.ChooseName(ChosenName.text))
        {
            LobbyNetwork.Instance.JoinLobby();
        }
        else
        {
            ErrorText.text = ChosenName.text + " is already being used.";
        }
    }
}
