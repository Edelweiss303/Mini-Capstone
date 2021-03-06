﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavesManager : Singleton<WavesManager>
{
    public List<MainSpawner> GameSpawners;
    public List<GameObject> WaveSpawn = new List<GameObject>();
    public int EnemyIncreasePerWave = 3;
    public int CurrentWave = 1;

    public int BigChaserSpawnIncreaseRoundFrequency = 3;
    public int SmallChaserSpawnRateIncreaseRoundFrequency = 4;
    public int SmallChaserSpawnIncreaseRoundFrequency = 2;
    public int SentrySpawnIncreaseRoundFrequency = 5;
    public int MinimumSpawnRateFrequency = 3;
    public int InterceptorSpawnIncreaseRoundFrequency = 6;
    public int CollectorSpawnIncreaseRoundFrequency = 10;

    public float TimeBetweenWaves = 30.0f;

    public int CurrentAmountSpawnedInWave = 0;
    public int MaxSpawnAmount = 15;
    public Text WaveNotificationText;

    public bool CurrentlyActive = true;

    private float timePassedSinceLastWaveEnded = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        initialize();
    }

    void initialize()
    {

    }

    // Update is called once per frame
    void Update()
    {
        WaveUpdate();
    }

    private void WaveUpdate()
    {
        WaveNotificationText.text = "Wave " + CurrentWave + System.Environment.NewLine + "Wave spawn: " + WaveSpawn.Count;
        if (CurrentlyActive)
        {
            if (CurrentAmountSpawnedInWave >= MaxSpawnAmount)
            {
                CurrentlyActive = false;
            }
        }
        else
        {
            timePassedSinceLastWaveEnded += Time.deltaTime;
            if(timePassedSinceLastWaveEnded > TimeBetweenWaves)
            {
                CurrentlyActive = true;
                CurrentWave++;
                MaxSpawnAmount += EnemyIncreasePerWave;
                CurrentAmountSpawnedInWave = 0;
                timePassedSinceLastWaveEnded = 0.0f;
                WaveSpawn.Clear();
                foreach (MainSpawner spawner in GameSpawners)
                {
                    if(CurrentWave % SmallChaserSpawnRateIncreaseRoundFrequency == 0)
                    {
                        if(spawner.ChaserSpawnRate > MinimumSpawnRateFrequency)
                        {
                            spawner.ChaserSpawnRate--;
                        }
                    }

                    if(CurrentWave % SmallChaserSpawnIncreaseRoundFrequency == 0)
                    {
                        spawner.SpawnableObjects.Add(AddressablesManager.Addressable_Tag.chaser);
                        spawner.SpawnableObjects.Add(AddressablesManager.Addressable_Tag.chaser);
                    }

                    if(CurrentWave % BigChaserSpawnIncreaseRoundFrequency == 0)
                    {
                        spawner.SpawnableObjects.Add(AddressablesManager.Addressable_Tag.bigchaser);
                    }

                    if (CurrentWave % SentrySpawnIncreaseRoundFrequency == 0)
                    {
                        spawner.SpawnableObjects.Add(AddressablesManager.Addressable_Tag.sentry);
                    }

                    if(CurrentWave % InterceptorSpawnIncreaseRoundFrequency == 0)
                    {
                        spawner.SpawnableObjects.Add(AddressablesManager.Addressable_Tag.interceptor);
                    }
                    if(CurrentWave % CollectorSpawnIncreaseRoundFrequency == 0)
                    {
                        spawner.SpawnableObjects.Add(AddressablesManager.Addressable_Tag.collector);
                    }

                    spawner.SpawnedEnemies.Clear();
                }
            }
        }

    }
}
