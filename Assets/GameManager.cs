using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public bool GameStatus = true;
    public GameObject PauseMenu;
    public Vector2 ScreenSize;
    public Text InputTest;

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        GameStatus = false;
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        GameUpdate();
    }

    void GameUpdate()
    {
        if (InputManager.Instance.Escape)
        {
            if (GameStatus)
            {
                Time.timeScale = 0.0f;
                GameStatus = false;
                PauseMenu.SetActive(true);
            }
            else
            {
                QuitGame();
            }
        }
        else if(GameStatus)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
        }

        InputTest.text = "Rotation Rate: " + (int)InputManager.Instance.CursorMovement.x + ", " + (int)InputManager.Instance.CursorMovement.y + ", " + (int)InputManager.Instance.CursorMovement.z;
    }

}
