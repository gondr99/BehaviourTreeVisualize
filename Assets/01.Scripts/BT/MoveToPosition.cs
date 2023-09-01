using System.Collections;
using System.Collections.Generic;
using BTVisual;
using UnityEngine;

public class MoveToPosition : ActionNode
{
    public float speed = 5;
    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;
    
    protected override void OnStart()
    {
        var agent = context.agent;
        agent.stoppingDistance = stoppingDistance;
        agent.speed = speed;
        agent.destination = blackboard.moveToPosition;
        agent.updateRotation = updateRotation;
        agent.acceleration = acceleration;
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        if (context.agent.pathPending) {
            return State.RUNNING;
        }

        if (context.agent.remainingDistance < tolerance) {
            return State.SUCCESS;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
            return State.FAILURE;
        }

        return State.RUNNING;
    }
}
