using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : EnemyBase
{
    public enum State
    {
        Moving, Collecting, Idle
    }
    public State CurrentState = State.Moving;
    public float InitialHeight;
    public List<ComponentBehaviour> Shields;
    public float CollectionTimeRequired = 3.0f;
    public float CheckCurrentGoalFrequency = 1.0f;
    public float RotationRate = 1.5f;
    public Vector3 Target;
    public float MoveSpeed = 3.0f;

    private FactoryCore core;
    private KeyValuePair<float, GameObject> currentTargetPickup;
    private ComponentBehaviour selectedComponent;
    private bool initialized = false;
    private Rigidbody rb;
    private float currentTimeCollecting = 0.0f;
    private float timeSinceLastGoalCheck = 0.0f;
    

    protected override void Start()
    {
        Type = EnemyType.collector;
        base.Start();
        core = GetComponentInChildren<FactoryCore>();
        rb = GetComponent<Rigidbody>();
        SetEnemyType();
        core.Colour = Colour;
        core.GetComponent<MeshRenderer>().material = EnemyMaterial;
        InitialHeight = transform.position.y;

        gameID = gameObject.GetInstanceID();
        EnemiesManager.Instance.addEnemy(gameID, gameObject, Type, Colour);
    }

    void Update()
    {
        if (!initialized)
        {
            Shields.AddRange(GetComponentsInChildren<ComponentBehaviour>());
            selectComponent();
            CurrentState = State.Idle;
            initialized = true;
        }

        MoveUpdate();
    }

    void MoveUpdate()
    {
        timeSinceLastGoalCheck += Time.deltaTime;
        if(CurrentState == State.Collecting)
        {
            transform.Rotate(0, RotationRate, 0);

            if(timeSinceLastGoalCheck > CheckCurrentGoalFrequency)
            {
                if(currentTargetPickup.Value == null)
                {
                    CurrentState = State.Idle;
                    return;
                }
            }

            //Collect that sucker
            currentTimeCollecting += Time.deltaTime;
            if(currentTimeCollecting > CollectionTimeRequired)
            {
                currentTimeCollecting = 0.0f;

                //Play a sound and destroy the pickup
                AudioManager.Instance.PlaySound("Collection");
                GameNetwork.Instance.ToPlayerQueue.Add("p:PilotCollectorDestroyPickup:" + currentTargetPickup.Key);
                GunnerController.Instance.PickupsMap.Remove(currentTargetPickup.Key);
                Destroy(currentTargetPickup.Value);
                getNewPickupTarget();
            }
            
        }
        else if(CurrentState == State.Moving)
        {
            if (movementIsComplete())
            {
                CurrentState = State.Collecting;
            }

            if (timeSinceLastGoalCheck > CheckCurrentGoalFrequency)
            {
                if (currentTargetPickup.Value == null)
                {
                    CurrentState = State.Idle;
                    return;
                }
                timeSinceLastGoalCheck = 0.0f;
            }

            transform.LookAt(Target);
            transform.position += (transform.forward * MoveSpeed * Time.deltaTime);
        }
        else if(CurrentState == State.Idle)
        {
            if(timeSinceLastGoalCheck > CheckCurrentGoalFrequency)
            {
                getNewPickupTarget();
                timeSinceLastGoalCheck = 0.0f;
            }
        }
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        transform.position = new Vector3(transform.position.x, InitialHeight, transform.position.z);

    }

    void selectComponent()
    {
        selectedComponent = Shields[UnityEngine.Random.Range(0, Shields.Count)];
        selectedComponent.Select(Colour, EnemyMaterial);
    }

    private void getNewPickupTarget()
    {
        float closestDistance = Mathf.Infinity, currentDistance = 0.0f;

        if(GunnerController.Instance.PickupsMap.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<float, GameObject> pickup in GunnerController.Instance.PickupsMap)
        {
            currentDistance = (transform.position - pickup.Value.transform.position).magnitude;
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                currentTargetPickup = pickup;
            }
        }

        Target = new Vector3(currentTargetPickup.Value.transform.position.x, transform.position.y, currentTargetPickup.Value.transform.position.z);
        CurrentState = State.Moving;
    }

    private bool movementIsComplete()
    {
        if((transform.position - Target).magnitude < 1.0f)
        {
            rb.velocity = Vector3.zero;
            CurrentState = State.Collecting;
            return true;
        }
        return false;
    }

    public void killSelectedComponent()
    {
        Shields.Remove(selectedComponent);
        Destroy(selectedComponent.gameObject);
        
        selectComponent();
    }

    public override void TakeDamage(float damage)
    {
        
    }

    public override void Die()
    {
        
    }

    public override bool IsAlive()
    {
        return true;
    }
}
