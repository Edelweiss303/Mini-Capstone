using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RetreatSteeringBehaviour : SteeringBehaviourBase
{
    public enum TargetType
    {
        Player, Friend, None
    }
    public TargetType targetType = TargetType.Player;

    private Transform targetTransform;
    private NavMeshPath path;
    private int currentPathTargetIndex = 0;
    private float timeSpentRepathing = 0.0f;

    public float RepathingCooldown = 0.5f;
    public float SetNewPathTargetDistance = 0.05f;

    private void Awake()
    {
        switch (targetType)
        {
            case TargetType.Player:
                targetTransform = FindObjectOfType<PlayerBehaviour>().gameObject.transform;
                break;
            case TargetType.Friend:
                targetTransform = FindObjectOfType<FriendController>().gameObject.transform;
                break;
        }
    }

    public void CalculatePath()
    {
        path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, (transform.position - targetTransform.position) * 3, NavMesh.AllAreas, path);
        if (path != null && path.corners.Length > 0)
        {
            target = path.corners[0];
        }
        currentPathTargetIndex = 0;
    }

    public override Vector3 calculateForce()
    {
        Vector3 desiredVelocity;

        if (targetTransform)
        {
            timeSpentRepathing += Time.deltaTime;

            if(timeSpentRepathing > RepathingCooldown)
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

            desiredVelocity = (target - transform.parent.position).normalized;
            desiredVelocity = desiredVelocity * steeringAgent.maxSpeed;
            return steeringAgent.velocity - desiredVelocity;
        }

        return Vector3.zero;
    }
}
