using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interceptor : EnemyBase
{
    public enum State
    {
        chasing, intercepting
    }
    public State CurrentState;

    public float Speed = 1.0f;
    public float Health = 3.0f;
    public float DeathTime = 4.0f;
    public bool Alive = true;
    public GameObject ExplosionPrefab;
    public string DeathAudioClipName;
    public float InterceptDistance = 5.0f;
    public float ChasingTimeLimit = 5.0f;

    private float currentTimeChasing = 0.0f;
    private float totalDyingTime = 0.0f;
    private MeshRenderer enemyRenderer;
    private BoxCollider enemyCollider;
    private MoveToSteeringBehaviour moveToSteeringBehaviour;
    private ChaseSteeringBehaviour chaseSteeringBehaviour;
    private SteeringAgent agent;

    protected override void Start()
    {
        agent = GetComponent<SteeringAgent>();
        enemyRenderer = GetComponent<MeshRenderer>();
        enemyCollider = GetComponent<BoxCollider>();
        moveToSteeringBehaviour = GetComponentInChildren<MoveToSteeringBehaviour>();
        chaseSteeringBehaviour = GetComponentInChildren<ChaseSteeringBehaviour>();
        moveToSteeringBehaviour.enabled = true;
        chaseSteeringBehaviour.enabled = false;
        moveToSteeringBehaviour.intercept(InterceptDistance);
        CurrentState = State.intercepting;
        if (moveToSteeringBehaviour.Complete)
        {
            moveToSteeringBehaviour.chase();
            CurrentState = State.chasing;
        }
        

        SetEnemyType();
        enemyRenderer.material = ColourManager.Instance.ProjectileMaterialMap[Colour];
        gameID = gameObject.GetInstanceID();
        EnemiesManager.Instance.addEnemy(gameID, gameObject, Type, Colour);
    }

    void Update()
    {
        EnemyUpdate();
        
    }

    private void EnemyUpdate()
    {
        if (Alive)
        {
            DamageEffectLoop(false);

            if(CurrentState == State.intercepting)
            {
                if (moveToSteeringBehaviour.Complete)
                {
                    moveToSteeringBehaviour.enabled = false;
                    chaseSteeringBehaviour.enabled = true;
                    CurrentState = State.chasing;
                }
            }
            else
            {
                currentTimeChasing += Time.deltaTime;
                if(currentTimeChasing > ChasingTimeLimit)
                {
                    currentTimeChasing = 0.0f;
                    moveToSteeringBehaviour.enabled = true;
                    chaseSteeringBehaviour.enabled = false;
                    moveToSteeringBehaviour.intercept(InterceptDistance);
                    CurrentState = State.intercepting;
                    if (moveToSteeringBehaviour.Complete)
                    {
                        moveToSteeringBehaviour.enabled = false;
                        chaseSteeringBehaviour.enabled = true;
                        CurrentState = State.chasing;
                    }
                }
            }
        }
        else
        {
            totalDyingTime += Time.deltaTime;

            if (totalDyingTime > DeathTime)
            {
                Destroy(this.gameObject);
            }
        }

    }

    override public void TakeDamage(float damage)
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
        enemyCollider.enabled = false;
        if (enemyRenderer)
        {
            enemyRenderer.enabled = false;
        }
        EnemiesManager.Instance.removeEnemy(gameID);

        //Create a death explosion effect and sound
        AudioManager.Instance.PlaySound(DeathAudioClipName);

        if (ExplosionPrefab)
        {
            Instantiate(ExplosionPrefab, this.transform.position, Quaternion.identity);
        }
        GunnerController.Instance.IncreaseScore(Score);
    }
}
