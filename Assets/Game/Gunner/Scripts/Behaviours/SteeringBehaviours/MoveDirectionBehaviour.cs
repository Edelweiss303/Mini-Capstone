using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirectionBehaviour : SteeringBehaviourBase
{
    public Vector3 MoveDirection;
    public float MaxDistance = 10.0f;

    public override Vector3 calculateForce()
    {
        target += MoveDirection;
        target.Normalize();
        target = target * steeringAgent.maxSpeed;
        return target - steeringAgent.velocity;
    }
}
