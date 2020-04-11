using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainSpawner : EnemyBase
{
    public float MinimumRotation = -0.1f;
    public float MaximumRotation = 2.0f;

    public float RespawnThreshold = 25.0f;
    public float Health = 10.0f;
    public Transform SpawnLocation;

    public GameObject ChaserPrefab;
    public int ChaserSpawnWeight;
    public float ChaserSpawnRate;
    public GameObject SentryPrefab;
    public int SentrySpawnWeight;
    public float SentrySpawnRate;
    public string SpawnerPowerDownSoundEffectName;

    private List<GameObject> spawnObjects = new List<GameObject>();
    private Dictionary<GameObject, float> spawnRateMap = new Dictionary<GameObject, float>();
    private float currentSpawnFrequency = 4.5f;
    private float timeSinceLastSpawn = 0.0f;
    private float timeRespawning = 0.0f;
    private EnemyBase.EnemyColour currentSpawnColour;
    private GameObject currentSpawnType;

    private bool isActive = true;
    private MeshRenderer spawnerRenderer;
    private List<MeshRenderer> nodeRenderers;


    // Start is called before the first frame update
    protected override void Start()
    {
        for(int i = 0; i < ChaserSpawnWeight; i++)
        {
            spawnObjects.Add(ChaserPrefab);
        }
        for(int i = 0; i < SentrySpawnWeight; i++)
        {
            spawnObjects.Add(SentryPrefab);
        }
        spawnRateMap.Add(ChaserPrefab, ChaserSpawnRate);
        spawnRateMap.Add(SentryPrefab, SentrySpawnRate);

        spawnerRenderer = GetComponent<MeshRenderer>();
        nodeRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
        nodeRenderers.Remove(spawnerRenderer);
        SetEnemyType();
        spawnerRenderer.material = EnemyMaterial;
        type = EnemyType.chaser;
        gameID = gameObject.GetInstanceID();
        setSpawnColour();
        setSpawnType();

        type = EnemyType.spawn;
        gameID = gameObject.GetInstanceID();
        EnemiesManager.Instance.addEnemy(gameID, gameObject, type, Colour);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            MoveUpdate();
            SpawnUpdate();
        }
        else
        {
            RespawnUpdate();
        }
    }

    private void MoveUpdate()
    {
        transform.Rotate(new Vector3(UnityEngine.Random.Range(MinimumRotation, MaximumRotation), UnityEngine.Random.Range(MinimumRotation, MaximumRotation), UnityEngine.Random.Range(MinimumRotation, MaximumRotation)));
    }

    private void SpawnUpdate()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if(timeSinceLastSpawn > currentSpawnFrequency)
        {
            //Spawn a new enemy
            GameObject newEnemy = Instantiate(currentSpawnType, SpawnLocation.position, Quaternion.identity);
            newEnemy.GetComponent<EnemyBase>().SetEnemyColour(currentSpawnColour);
            setSpawnColour();
            setSpawnType();
            timeSinceLastSpawn = 0.0f;
        }

    }

    private void setSpawnType()
    {
        currentSpawnType = spawnObjects[UnityEngine.Random.Range(0, spawnObjects.Count)];
        currentSpawnFrequency = spawnRateMap[currentSpawnType];
    }

    private void setSpawnColour()
    {
        currentSpawnColour = GunnerController.Instance.GetRandomizedEnemyColour();
        Material currentSpawnMaterial = ColourManager.Instance.ShieldMaterialMap[currentSpawnColour];
        foreach(Renderer nodeRenderer in nodeRenderers)
        {
            nodeRenderer.material = currentSpawnMaterial;
        }
    }

    private void RespawnUpdate()
    {
        timeRespawning += Time.deltaTime;

        if(timeRespawning > RespawnThreshold)
        {
            spawnerRenderer.material = ColourManager.Instance.EnemyMaterialMap[Colour];
            isActive = true;
            timeRespawning = 0.0f;
        }
    }

    public override void TakeDamage(float damage)
    {
        ShowDamageEffect();
        Health -= damage;

        if (!IsAlive())
        {
            Die();
        }
    }

    public override bool IsAlive()
    {
        return Health > 0;
    }

    public override void Die()
    {
        isActive = false;
        AudioManager.Instance.PlaySound(SpawnerPowerDownSoundEffectName);
    }
}
