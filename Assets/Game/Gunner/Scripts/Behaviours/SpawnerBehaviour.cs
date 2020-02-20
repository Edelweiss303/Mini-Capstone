using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehaviour : MonoBehaviour
{
    public List<GameObject> SpawnPrefabs;
    public float SpawnRate = 0.5f;
    public Transform SpawnContainer;
    public float SpawnHeightOffsetRange, SpawnWidthOffsetRange, SpawnDepthOffsetRange;
    public float SpawnLimit = 10.0f;

    private float timeSinceLastSpawn = 0.0f;
    private List<GameObject> spawns = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        SpawningUpdate();
    }

    private void SpawningUpdate()
    {
        if (SpawnPrefabs.Count > 0)
        {
            if(timeSinceLastSpawn > SpawnRate && spawns.Count < SpawnLimit)
            {
                Vector3 spawnPoint = new Vector3(
                    Random.Range(-SpawnWidthOffsetRange, SpawnWidthOffsetRange), 
                    Random.Range(-SpawnHeightOffsetRange, SpawnHeightOffsetRange), 
                    Random.Range(-SpawnDepthOffsetRange, SpawnDepthOffsetRange)
                );
                int spawnIndex = Random.Range(0, SpawnPrefabs.Count);
                GameObject newSpawn = Instantiate(SpawnPrefabs[spawnIndex], spawnPoint + transform.position, Quaternion.identity);
                EnemiesManager.Instance.addEnemy(newSpawn);
                spawns.Add(newSpawn);
                
                timeSinceLastSpawn = 0.0f;
            }

            timeSinceLastSpawn += Time.deltaTime;
        }

        for(int i = spawns.Count - 1; i >=0; i--)
        {
            if (!spawns[i])
            {
                spawns.RemoveAt(i);
            }
        }
        
    }
}
