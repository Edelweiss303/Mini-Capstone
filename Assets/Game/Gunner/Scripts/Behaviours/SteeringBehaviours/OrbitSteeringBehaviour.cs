using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrbitSteeringBehaviour : SteeringBehaviourBase
{
    public bool Colliding = false;

    public enum TargetType
    {
        Player, Friend, None
    }
    public TargetType targetType = TargetType.Player;
    public float Speed = 2.0f;

    private int directionalSign = 1;
    private Transform targetTransform;
    private Vector3 directionalAxis = Vector3.up;

    // Start is called before the first frame update
    void Start()
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

    public override Vector3 calculateForce()
    {
        if (targetTransform)
        {
            Vector3 offsetFromCenter = transform.position - targetTransform.position;
            float radius = offsetFromCenter.magnitude;
            Vector3 travelDirection = Vector3.Cross(directionalAxis * directionalSign, offsetFromCenter).normalized;
            Vector3 desiredVelocity = (radius * Speed * Mathf.Deg2Rad * travelDirection).normalized;

            desiredVelocity = desiredVelocity * steeringAgent.maxSpeed;
            return desiredVelocity - steeringAgent.velocity;
        }

        return Vector3.zero;
    }
}
