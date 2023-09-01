using System.Collections;
using System.Collections.Generic;
using BTVisual;
using UnityEngine;

public class ChaseToEnemy : ActionNode
{
    public float checkTime = 0.2f; //0.2초마다 체크
    public float chaseRange = 14f;
    public float successRange = 6f;
    private float _timer = 0;
    
    
    protected override void OnStart()
    {
        context.agent.destination = blackboard.enemySpotPosition;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (Time.time > _timer + checkTime)
        {
            if (!CheckEmeny()) //적이 범위밖으로 벗어남.
            {
                return State.FAILURE;
            }
            context.agent.destination = blackboard.enemySpotPosition;
            _timer = Time.time;
        }
        
        //남은 거리가 success안쪽이면 성공
        if (context.agent.remainingDistance < successRange) {
            return State.SUCCESS;
        }

        return State.RUNNING;
    }

    private bool CheckEmeny()
    {
        Collider[] results = new Collider[1]; //한개만
        var size = Physics.OverlapSphereNonAlloc(context.transform.position, chaseRange, results, blackboard.whatIsEnemy);
        if (size >= 1)
        {
            blackboard.enemySpotPosition = results[0].transform.position;
            return true;
        }

        return false;
    }
}
