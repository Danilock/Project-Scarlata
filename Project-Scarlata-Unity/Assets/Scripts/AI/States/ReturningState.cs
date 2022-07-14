using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AI
{
    public class ReturningState : State<TargetDetection>
    {
        public override void OnEnter(TargetDetection entity)
        {
            entity.Animator.SetBool("Run", true);
        }

        public override void OnUpdate(TargetDetection entity)
        {
            entity.MoveTo(entity.InitialPosition);

            if(entity.ReachedDistance(entity.InitialPosition))
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