using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static GunnerController;

public class PilotController : Singleton<PilotController>
{
    public float PickupSpawnFrequency = 8.0f;
    public float PickupSpawnRange = 4.0f;
    public List<GameObject> PickupSpawnLocations;
    public GameObject PickupPrefab;
    public Transform PickupsContainerTransform;

    private float timeSinceLastPickupSpawn = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PickupUpdate();
    }


    void PickupUpdate()
    {
        timeSinceLastPickupSpawn += Time.deltaTime;
        if(timeSinceLastPickupSpawn > PickupSpawnFrequency)
        {
            Vector3 point;

            //Choose a pickupspawnpoint to originate from
            Vector3 spawnPointPosition = PickupSpawnLocations[UnityEngine.Random.Range(0, PickupSpawnLocations.Count)].transform.position;
            point = new Vector3(UnityEngine.Random.Range(-PickupSpawnRange, PickupSpawnRange), 0, UnityEngine.Random.Range(-PickupSpawnRange, PickupSpawnRange)) + spawnPointPosition;

            //Spawn pickup
            Instantiate(PickupPrefab, point, Quaternion.identity, PickupsContainerTransform);

            //Spawn syncing with gunner
            

            timeSinceLastPickupSpawn = 0.0f;
        }
    }
}
