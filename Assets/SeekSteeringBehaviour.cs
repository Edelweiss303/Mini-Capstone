using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SeekSteeringBehaviour : SteeringBehaviourBase
{
    public Vector3 EndGoal = Vector3.zero;
    private NavMeshPath path;
    private int currentPathTargetIndex = 0;
    private float timeSpentRepathing = 0.0f;

    public float RepathingCooldown = 2.0f;
    public float SetNewPathTargetDistance = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        CalculatePath();
    }

    public override Vector3 calculateForce()
    {

        timeSpentRepathing += Time.deltaTime;

        if (timeSpentRepathing > RepathingCooldown)
        {
            timeSpentRepathing = 0.0f;
            CalculatePath();
        }

        if (path != null)
        {
            if (currentPathTargetIndex < path.corners.Length - 1)
            {
                if ((new Vector3(path.corners[currentPathTargetIndex].x, 0, path.corners[currentPathTargetIndex].z) -
                new Vector3(transform.position.x, 0, transform.position.z)).magnitude < SetNewPathTargetDistance)
                {
                    currentPathTargetIndex++;
                    target = path.corners[currentPathTargetIndex];
                }
            }
        }

        Vector3 desiredVelocity = (target - transform.parent.position).normalized;
        desiredVelocity = desiredVelocity * steeringAgent.maxSpeed;
        return desiredVelocity - steeringAgent.velocity;
    }

    public void CalculatePath()
    {
        path = new NavMeshPath();
        
        NavMesh.CalculatePath(transform.position, EndGoal, NavMesh.AllAreas, path);
        
        if (path != null && path.corners.Length > 0)
        {
            target = path.corners[0];
        }
        currentPathTargetIndex = 0;
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            for(int i = 1; i < path.corners.Length; i++)
            {
                Debug.DrawLine(path.corners[i - 1], path.corners[i], Color.red);
            }
        }
    }
}
