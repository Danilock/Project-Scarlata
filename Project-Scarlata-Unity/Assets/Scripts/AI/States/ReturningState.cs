using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AI
{
    public class ReturningState : State<AIAgent>
    {
        public override void OnEnter(AIAgent entity)
        {
            entity.Animator.SetBool("Run", true);
        }

        public override void OnUpdate(AIAgent entity)
        {
            entity.MoveTo(entity.InitialPosition);

            if(entity.ReachedDistance(entity.InitialPosition))
            {
                entity.StateMachine.SetState<IdleState>();
            }
            if (entity.TargetDetection.IsDetectingATarget())
            {
                entity.StateMachine.SetState<FollowState>();
            }
        }
    }
}