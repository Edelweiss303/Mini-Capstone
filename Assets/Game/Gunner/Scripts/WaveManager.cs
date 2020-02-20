using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaveManager : Singleton<WaveManager>
{
    public List<Wave> GameWaves = new List<Wave>();
    public Wave ActiveWave;
    public Text WaveNotification;

    private int activeWaveIndex = 0;
    private bool gameIsOver = false;

    // Start is called before the first frame update
    void Start()
    {
        if(GameWaves.Count > 0)
        {
            activeWaveIndex = 0;
            ActiveWave = GameWaves[activeWaveIndex];
            ActiveWave.activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            WaveNotification.text = "You win!!!!";
            SceneManager.LoadScene(0);
            return;
        }
        if (ActiveWave)
        {
            if (ActiveWave.IsComplete)
            {
                ActiveWave = null;
                activeWaveIndex++;
                if (GameWaves.Count > activeWaveIndex)
                {
                    ActiveWave = GameWaves[activeWaveIndex];
                    ActiveWave.activate();
                }
                else
                {
                    gameIsOver = true;
                }
            }
            else
            {
                WaveNotification.text = ActiveWave.Notification;
                WaveNotification.color = ActiveWave.NotificationColour;
            }
        }
    }

    public void EnemyWasKilled()
    {
        ActiveWave.enemyWasKilled();
    }
}
