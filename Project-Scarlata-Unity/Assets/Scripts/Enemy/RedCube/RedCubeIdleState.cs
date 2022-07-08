using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.Enemies
{
    public class RedCubeIdleState : State<RedCubeController>
    {
        public override void OnEnter(RedCubeController entity)
        {

        }

        public override void OnUpdate(RedCubeController entity)
        {
            if (entity.TargetDetection.IsDetectingATarget())
            {
                entity.StateMachine.SetState<RedCubeFollowState>();
            }
        }
    }
}