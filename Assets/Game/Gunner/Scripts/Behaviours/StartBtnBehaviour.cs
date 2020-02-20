using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtnBehaviour : MonoBehaviour
{
    public void StartBtn_Click()
    {
        SceneManager.LoadScene(1);
    }
}
