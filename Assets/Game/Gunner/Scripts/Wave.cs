using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public int NumberOfEnemiesToKill = 20;
    public GameObject SpawnerObject;
    public BossBehaviour BossBehaviour;
    public bool IsComplete = false;
    public string Notification = "";
    public string Name = "Wave One";
    public Color NotificationColour = Color.cyan;
    public Color SpawningColour = Color.cyan;
    public Color BossColour = Color.red;

    private int numberOfEnemiesKilled = 0;

    private void Update()
    {
        if(NumberOfEnemiesToKill > numberOfEnemiesKilled)
        {
            Notification = Name + ": " + (NumberOfEnemiesToKill - numberOfEnemiesKilled).ToString() + " Enemies Remaining.";
            NotificationColour = SpawningColour;
        }
        else if (BossBehaviour && BossBehaviour.IsAlive)
        {
            if (SpawnerObject.activeSelf)
            {
                SpawnerObject.SetActive(false);
            }
            if (!BossBehaviour.gameObject.activeSelf)
            {
                BossBehaviour.gameObject.SetActive(true);
            }
            Notification = "BOSS BATTLE!!!";
            NotificationColour = BossColour;
        }
        else
        {
            if (SpawnerObject.activeSelf)
            {
                SpawnerObject.SetActive(false);
                
            }
            if (BossBehaviour)
            {
                BossBehaviour.gameObject.SetActive(false);
            }

            Transform eManagerTransform = EnemiesManager.Instance.transform;
            foreach (Transform child in eManagerTransform)
            {
                GameObject.Destroy(child.gameObject);
            }

            IsComplete = true;
        }
    }

    public void activate()
    {
        if (SpawnerObject)
        {
            SpawnerObject.SetActive(true);
        }
    }

    public void enemyWasKilled()
    {
        numberOfEnemiesKilled++;
    }

}
