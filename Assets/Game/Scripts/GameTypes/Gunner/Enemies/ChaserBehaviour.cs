using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaserBehaviour : EnemyBase
{
    public float Speed = 1.0f;
    public float Health = 3.0f;
    public float DeathTime = 4.0f;
    public float RetreatingTime = 1.5f;
    public bool Alive = true;

    public GameObject ExplosionPrefab;
    public string DeathAudioClipName;

    private float timeRetreating = 0.0f;
    private float totalDyingTime = 0.0f;
    private MeshRenderer enemyRenderer;
    private BoxCollider enemyCollider;
    private ChaseSteeringBehaviour chaseSteeringBehaviour;
    private RetreatSteeringBehaviour retreatSteeringBehaviour;
    private SteeringAgent agent;

    protected override void Start()
    {

        timeRetreating = 0.0f;
        agent = GetComponent<SteeringAgent>();
        enemyRenderer = GetComponent<MeshRenderer>();
        enemyCollider = GetComponent<BoxCollider>();
        chaseSteeringBehaviour = GetComponentInChildren<ChaseSteeringBehaviour>();
        retreatSteeringBehaviour = GetComponentInChildren<RetreatSteeringBehaviour>();
        retreatSteeringBehaviour.enabled = false;
        chaseSteeringBehaviour.enabled = true;


        SetEnemyType();
        enemyRenderer.material = EnemyMaterial;
        type = EnemyType.chaser;
        gameID = gameObject.GetInstanceID();
        EnemiesManager.Instance.addEnemy(gameID, gameObject, type, Colour);
    }

    void Update()
    {
        EnemyUpdate();
    }

    private void EnemyUpdate()
    {
        if (Alive)
        {
            if(timeRetreating > 0.0f)
            {
                timeRetreating += Time.deltaTime;
                if(timeRetreating > RetreatingTime)
                {
                    timeRetreating = 0.0f;
                    retreatSteeringBehaviour.enabled = false;
                    chaseSteeringBehaviour.enabled = true;
                }
            }

            DamageEffectLoop(false);
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
        retreatSteeringBehaviour.enabled = true;
        retreatSteeringBehaviour.target = chaseSteeringBehaviour.target;
        agent.velocity = Vector3.zero;
        retreatSteeringBehaviour.CalculatePath();
        chaseSteeringBehaviour.enabled = false;
        timeRetreating = 0.0f;
        timeRetreating += Time.deltaTime;
        
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
    }

    //public void OnTriggerEnter(Collider collider)
    //{
    //    PlayerBehaviour pBehaviour = collider.gameObject.GetComponent<PlayerBehaviour>();
    //    if (pBehaviour)
    //    {
    //        pBehaviour.TakeDamage(Damage);
    //        Die();
    //    }
    //}

}
