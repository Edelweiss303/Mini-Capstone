using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehaviour : MonoBehaviour
{
    public List<GameObject> SpawnPrefabs;
    public float SpawnRate = 0.5f;
    public Transform SpawnContainer;

    private BoxCollider spawnContainer;
    private float timeSinceLastSpawn = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        spawnContainer = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        SpawningUpdate();
    }

    private void SpawningUpdate()
    {
        if (spawnContainer && SpawnPrefabs.Count > 0)
        {
            if(timeSinceLastSpawn > SpawnRate)
            {
                Vector3 spawnPoint = new Vector3(
                    Random.Range(spawnContainer.bounds.min.x, spawnContainer.bounds.max.x), 
                    Random.Range(spawnContainer.bounds.min.y, spawnContainer.bounds.max.y), 
                    Random.Range(spawnContainer.bounds.min.z, spawnContainer.bounds.max.z)
                );
                int spawnIndex = Random.Range(0, SpawnPrefabs.Count);
                Instantiate(SpawnPrefabs[spawnIndex], spawnPoint, Quaternion.identity, SpawnContainer);
                timeSinceLastSpawn = 0.0f;
            }

            timeSinceLastSpawn += Time.deltaTime;
            
        }
        
    }
}
