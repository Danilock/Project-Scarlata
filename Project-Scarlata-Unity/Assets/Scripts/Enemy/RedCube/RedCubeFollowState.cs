using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.Enemies
{
    public class RedCubeFollowState : State<RedCubeController>
    {
        public override void OnEnter(RedCubeController entity)
        {
            entity.NavAgent.PathTo(entity.TargetDetection.Target.position);
        }

        public override void OnUpdate(RedCubeController entity)
        {
            if (!entity.TargetDetection.IsDetectingATarget())
            {
                entity.NavAgent.ForceStop();
                entity.StateMachine.SetState<RedCubeIdleState>();
                return;
            }

            entity.NavAgent.PathTo(entity.TargetDetection.Target.position);
        }
    }
}