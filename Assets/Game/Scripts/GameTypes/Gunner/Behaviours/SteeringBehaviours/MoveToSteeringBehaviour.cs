using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToSteeringBehaviour : SteeringBehaviourBase
{
    private Transform targetObjectTransform;
    private Vector3 targetPosition;
    private NavMeshPath path;
    private int currentPathTargetIndex = 0;
    private PlayerBehaviour pBehaviour;

    public float SetNewPathTargetDistance = 0.1f;
    public float CompletionDistanceThreshold = 0.2f;
    public bool Complete = false;

    // Start is called before the first frame update
    void Start()
    {
        pBehaviour = FindObjectOfType<PlayerBehaviour>();
        targetObjectTransform = pBehaviour.gameObject.transform;
        
    }

    public void intercept(float distance)
    {
        if (pBehaviour == null)
        {
            pBehaviour = FindObjectOfType<PlayerBehaviour>();
        }
        if (targetObjectTransform == null)
        {
            targetObjectTransform = pBehaviour.gameObject.transform;
        }


        targetPosition = pBehaviour.Velocity.normalized * distance + targetObjectTransform.position;
        CalculatePath();

    }

    public void chase()
    {
        targetPosition = targetObjectTransform.position;
        CalculatePath();
    }

    public override Vector3 calculateForce()
    {
        Vector3 velocity = Vector3.zero;
        if (!Complete)
        {
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

            if((target - transform.parent.position).magnitude < CompletionDistanceThreshold)
            {
                Complete = true;
                return velocity;
            }

            Vector3 desiredVelocity = (target - transform.parent.position).normalized;
            desiredVelocity = desiredVelocity * steeringAgent.maxSpeed;
            velocity = desiredVelocity - steeringAgent.velocity;


        }

        return velocity;
       
    }

    public void CalculatePath()
    {
        Complete = false;
        path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);

        if (path != null)
        {
            if (path.corners.Length == 0)
            {
                Complete = true;
                return;
            }
            target = path.corners[0];
        }
        else
        {
            Complete = true;
            return;
        }
        currentPathTargetIndex = 0;
    }
}
