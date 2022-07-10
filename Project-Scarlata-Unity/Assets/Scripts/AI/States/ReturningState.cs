using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AI
{
    public class ReturningState : State<TargetDetection>
    {
        public override void OnUpdate(TargetDetection entity)
        {
            entity.MoveTo(entity.InitialPosition);

            if(Vector3.Distance(entity.transform.position, entity.InitialPosition) < .1f)
            {
                entity.StateMachine.SetState<IdleState>();
            }
            if (entity.IsDetectingATarget())
            {
                entity.StateMachine.SetState<FollowState>();
            }
        }
    }
}