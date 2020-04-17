using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skulker : EnemyBase
{
    public float MoveSpeed = 3.0f;
    public Vector3 CurrentMoveDirection = new Vector3(0, 0, 0);
    public LayerMask LevelMask, LevelBoundsMask;
    public GameObject Spawner;
    public GameObject Head;
    public float MaxPartInterpolation = 0.2f;
    public float PartOscillationFrequency = 0.1f;
    public float DirectionChangeFrequency = 5.0f;
    public bool Alive = true;
    public float Health = 2.0f;

    public GameObject ExplosionPrefab;
    public string DeathAudioClipName;

    public enum State
    {
        skulk, retreat
    }
    public State SkulkState;
    public float RetreatDistance = 1.0f;


    private Vector3 partOriginPosition;
    private List<MeshRenderer> enemyRenderers;
    private float timeSinceLastPartOscillation = 0.0f;
    private float timeSinceLastDirectionChange = 0.0f;
    private bool isInitialized = false;

    protected override void Start()
    {
        enemyRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
        int direction = UnityEngine.Random.Range(0, 4);
        CurrentMoveDirection.y = direction * 90;
        partOriginPosition = transform.position - Head.transform.position;

        SetEnemyType();
        foreach(MeshRenderer enemyRenderer in enemyRenderers)
        {
            enemyRenderer.material = EnemyMaterial;
        }

        gameID = gameObject.GetInstanceID();
        EnemiesManager.Instance.addEnemy(gameID, gameObject, Type, Colour);
    }

    private void initialize()
    {
        Spawner = WavesManager.Instance.GameSpawners.Single(s => s.SpawnedEnemies.Contains(gameObject)).SpawnLocation.gameObject;
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized)
        {
            initialize();
        }

        timeSinceLastPartOscillation += Time.deltaTime;
        if (timeSinceLastPartOscillation > PartOscillationFrequency)
        {
            timeSinceLastPartOscillation = 0.0f;
            Head.transform.position = transform.position + partOriginPosition + Head.transform.right * UnityEngine.Random.Range(-MaxPartInterpolation, MaxPartInterpolation);
        }

        if (SkulkState == State.skulk)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, CurrentMoveDirection.y, transform.eulerAngles.z);
            

            timeSinceLastDirectionChange += Time.deltaTime;

            if (timeSinceLastDirectionChange > DirectionChangeFrequency)
            {
                timeSinceLastDirectionChange = 0.0f;
                int direction = UnityEngine.Random.Range(0, 2) * 2 - 1;
                CurrentMoveDirection.y += direction * 25;
            }
        }
        else
        {
            transform.LookAt(new Vector3(Spawner.transform.position.x, transform.position.y, Spawner.transform.position.z));

            if((transform.position - Spawner.transform.position).magnitude < RetreatDistance)
            {
                SkulkState = State.skulk;
            }
        }

        transform.position += transform.forward * MoveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(LevelMask == (LevelMask | (1 << other.gameObject.layer)))
        {
            timeSinceLastDirectionChange = 0.0f;
            int direction = UnityEngine.Random.Range(0, 2) * 2 - 1;
            CurrentMoveDirection.y += direction * 25;
        }
        else if(LevelBoundsMask == (LevelBoundsMask | (1 << other.gameObject.layer)))
        {
            //Return to spawner
            SkulkState = State.retreat;
        }

        PlayerBehaviour pBehaviour = other.gameObject.GetComponent<PlayerBehaviour>();
        if (pBehaviour)
        {
            pBehaviour.TakeDamage(Damage);
            Die();
        }


    }

    public override void TakeDamage(float damage)
    {
        ShowDamageEffect();
        Health -= damage;

        if (Health <= 0)
        {
            Die();
        }
    }

    public override bool IsAlive()
    {
        return Alive;
    }

    public override void Die()
    {
        Alive = false;
        foreach (MeshRenderer enemyRenderer in enemyRenderers)
        {
            enemyRenderer.enabled = false;
        }

        EnemiesManager.Instance.removeEnemy(gameID);
        AudioManager.Instance.PlaySound(DeathAudioClipName);

        if (ExplosionPrefab)
        {
            Instantiate(ExplosionPrefab, this.transform.position, Quaternion.identity);
        }
        GunnerController.Instance.IncreaseScore(Score);
    }
}
