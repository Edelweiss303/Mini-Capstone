using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseSteeringBehaviour : SteeringBehaviourBase
{
    private Transform targetTransform;
    private NavMeshPath path;
    private int currentPathTargetIndex = 0;
    private float timeSpentRepathing = 0.0f;

    public float RepathingCooldown = 2.0f;
    public float SetNewPathTargetDistance = 0.1f;

    // Start is called before the first frame update
    void Start()
    { 
        targetTransform = FindObjectOfType<PlayerBehaviour>().gameObject.transform;
        CalculatePath();
    }

    public override Vector3 calculateForce()
    {
        if (targetTransform)
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

        return Vector3.zero;
    }

    void CalculatePath()
    {
        path = new NavMeshPath();
        Vector3 test = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);
        NavMesh.CalculatePath(transform.position, test, NavMesh.AllAreas, path);
        if (path != null && path.corners.Length > 0)
        {
            target = path.corners[0];
        }
        currentPathTargetIndex = 0;
    }
}
