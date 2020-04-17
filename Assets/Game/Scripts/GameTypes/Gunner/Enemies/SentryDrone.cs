using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryDrone : EnemyBase
{
    enum SentryState
    {
        Idle, Approach, Wait, Charge, Strafe, Retreat
    }
    private SentryState sState;

    public List<GameObject> DroneComponents;

    public float RotationRate = 1.0f;
    public float StrafeRate = 1.0f;
    public float MoveSpeed = 1.0f;
    public float LookAtSpeed = 1.0f;
    public float Health = 3.0f;

    public float DeathTime = 4.0f;
    public GameObject ComponentHolder, CannonObject;
    public GameObject ExplosionPrefab;

    public float AggroRange = 28.0f;
    public string DeathAudioClipName;

    public float initialHeight = 0.0f;

    private SentryShooting sentryShooting;
    private Transform pTransform;
    private PlayerBehaviour pBehaviour;
    private float totalDyingTime = 0.0f;
    private ComponentBehaviour selectedDrone;
    private ChaseSteeringBehaviour chaseSteeringBehaviour;
    private SteeringAgent steeringAgent;
    private bool isAlive = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        Type = EnemyType.sentry;
        base.Start();
        pBehaviour = FindObjectOfType<PlayerBehaviour>();
        pTransform = pBehaviour.gameObject.transform;
        chaseSteeringBehaviour = GetComponentInChildren<ChaseSteeringBehaviour>();
        steeringAgent = GetComponent<SteeringAgent>();
        int droneSelectionIndex = UnityEngine.Random.Range(0, DroneComponents.Count - 1);
        selectedDrone = DroneComponents[droneSelectionIndex].GetComponent<ComponentBehaviour>();
        sentryShooting = GetComponentInChildren<SentryShooting>();
        initialHeight = transform.position.y;
        SetEnemyType();
        if (selectedDrone)
        {
            selectedDrone.Select(Colour, EnemyMaterial);
        }
        
        sState = SentryState.Idle;
        gameID = gameObject.GetInstanceID();
        EnemiesManager.Instance.addEnemy(gameID, gameObject, Type, Colour);
    }

    // Update is called once per frame
    void Update()
    {
        SentryUpdate();
    }

    private void SentryUpdate()
    {
        if (IsAlive())
        {
            SetSentryState();

            switch (sState)
            {
                case (SentryState.Approach):
                    chaseSteeringBehaviour.enabled = true;
                    ComponentHolder.transform.rotation = transform.rotation;
                    break;
                case (SentryState.Charge):
                    chaseSteeringBehaviour.enabled = false;
                    steeringAgent.velocity = Vector3.zero;
                    ComponentHolder.transform.Rotate(0, 0, RotationRate);
                    sentryShooting.Charge(pTransform);
                    break;
                case (SentryState.Strafe):
                    chaseSteeringBehaviour.enabled = false;
                    steeringAgent.velocity = Vector3.zero;
                    Strafe();
                    break;
            }
        }
        else
        {
            totalDyingTime += Time.deltaTime;

            if (totalDyingTime > DeathTime)
            {
                Destroy(this.gameObject);
                //WaveManager.Instance.EnemyWasKilled();
            }
        }
       // transform.position = new Vector3(transform.position.x, initialHeight, transform.position.z);
    }

    private void SetSentryState()
    {
        if(pTransform != null)
        {
            float distanceFromPlayer = (pTransform.position - transform.position).magnitude;

            if (distanceFromPlayer > AggroRange)
            {
                sState = SentryState.Idle;
            }
            else if (!sentryShooting.IsInRange(distanceFromPlayer))
            {
                sState = SentryState.Approach;
            }
            else
            {
                if (!sentryShooting.IsRecharged)
                {
                    sState = SentryState.Strafe;
                }
                else
                {
                    sState = SentryState.Charge;
                }
            }
        }

    }

    private void Strafe()
    {
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3((pTransform.position - transform.position).x, 0, (pTransform.position - transform.position).z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, LookAtSpeed * Time.deltaTime);

       // transform.Translate(transform.right * Time.deltaTime * StrafeRate);
        sentryShooting.Recharge();
    }

    public override void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        isAlive = false;
        chaseSteeringBehaviour.enabled = false;
        foreach (GameObject component in DroneComponents)
        {
            component.SetActive(false);
        }
        CannonObject.SetActive(false);
        //Create a death explosion effect and sound
        AudioManager.Instance.PlaySound(DeathAudioClipName);

        if (ExplosionPrefab)
        {
            Instantiate(ExplosionPrefab, this.transform.position, Quaternion.identity);
        }
        GunnerController.Instance.IncreaseScore(Score);
    }



    public override bool IsAlive()
    {
        return isAlive;
    }


}
