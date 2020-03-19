using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryMachine : EnemyBase
{
    public enum FactoryState
    {
        Moving, Spawning
    }
    public FactoryState State = FactoryState.Moving;

    public List<ComponentBehaviour> Shields;
    public float ChangeDestinationFrequency = 3.0f;
    public float RotationRate = 1.5f;


    private ComponentBehaviour selectedComponent;
    private bool initialized = false;
    private SeekSteeringBehaviour seekSteeringBehaviour;
    private SteeringAgent steeringAgent;
    private Rigidbody rb;
    private float timeSinceLastChangeDestinationCheck = 0.0f;

    void Start()
    {
        seekSteeringBehaviour = GetComponentInChildren<SeekSteeringBehaviour>();
        steeringAgent = GetComponent<SteeringAgent>();
        rb = GetComponent<Rigidbody>();
        seekSteeringBehaviour.enabled = false;
        EnemiesManager.Instance.addEnemy(gameObject);
    }

    void Update()
    {
        if (!initialized)
        {
            Shields.AddRange(GetComponentsInChildren<ComponentBehaviour>());
            selectComponent();
            checkForDestinationChange();
            initialized = true;
        }

        MoveUpdate();
    }

    void MoveUpdate()
    {
        if(State == FactoryState.Spawning)
        {
            transform.Rotate(0, RotationRate, 0);
        }

        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        timeSinceLastChangeDestinationCheck += Time.deltaTime;
        if(timeSinceLastChangeDestinationCheck > ChangeDestinationFrequency)
        {
            timeSinceLastChangeDestinationCheck = 0.0f;

            checkForDestinationChange();
        }
    }

    void selectComponent()
    {
        selectedComponent = Shields[UnityEngine.Random.Range(0, Shields.Count)];
        selectedComponent.Select();
    }

    private void checkForDestinationChange()
    {
        Vector3 playerPosition = GameManager.Instance.PlayerObject.transform.position;
        Vector3 furthestPosition = Vector3.zero;
        float furthestDistance = 0.0f, currentDistance = 0.0f;


        foreach (Transform markerTransform in GameManager.Instance.FactoryMarkers)
        {
            currentDistance = (playerPosition - markerTransform.position).magnitude;
            if (currentDistance > furthestDistance)
            {
                furthestDistance = currentDistance;
                furthestPosition = markerTransform.position;
            }
        }

        if((transform.position - furthestPosition).magnitude < 1.0f 
            || GameManager.Instance.FactoryMarkers.Count == 0)
        {
            seekSteeringBehaviour.enabled = false;
            steeringAgent.velocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            State = FactoryState.Spawning;
        }
        else
        {
            seekSteeringBehaviour.enabled = true;
            seekSteeringBehaviour.EndGoal = furthestPosition;
            seekSteeringBehaviour.CalculatePath();
            State = FactoryState.Moving;
        }
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
