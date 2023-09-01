using BTVisual;
using UnityEngine;

public class CheckRange : ActionNode
{
    public float range;
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        Collider[] results = new Collider[1]; //한개만
        var size = Physics.OverlapSphereNonAlloc(context.transform.position, range, results, blackboard.whatIsEnemy);
        //거리상에 적으로 설정한 녀석이 있다면
        if (size >= 1)
        {
            blackboard.enemySpotPosition = results[0].transform.position;
            return State.SUCCESS;
        }
        return State.FAILURE;
    }
}
