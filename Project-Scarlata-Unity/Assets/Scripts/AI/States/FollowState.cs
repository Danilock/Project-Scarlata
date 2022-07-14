using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AI
{
    public class FollowState : State<AIAgent>
    {
        public override void OnEnter(AIAgent entity)
        {
            entity.Animator.SetBool("Run", true);
        }

        public override void OnUpdate(AIAgent entity)
        {
            entity.MoveTo(entity.TargetDetection.Target.position);

            if (!entity.TargetDetection.IsDetectingATarget())
            {
                if (entity.ReturnToInitialPosition)
                {
                    entity.StateMachine.SetState<ReturningState>();
                    return;
                }

                entity.StateMachine.SetState<IdleState>();
            }
        }
    }
}