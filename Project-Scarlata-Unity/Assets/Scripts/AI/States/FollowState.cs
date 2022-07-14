using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AI
{
    public class FollowState : State<TargetDetection>
    {
        public override void OnEnter(TargetDetection entity)
        {
            entity.Animator.SetBool("Run", true);
        }

        public override void OnUpdate(TargetDetection entity)
        {
            entity.MoveTo(entity.Target.position);

            if (!entity.IsDetectingATarget())
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