using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AI
{
    public class IdleState : State<AIAgent>
    {
        public override void OnEnter(AIAgent entity)
        {
            entity.Animator.SetBool("Run", false);
            entity.CanMove = true;
        }

        public override void OnUpdate(AIAgent entity)
        {
            if (entity.TargetDetection.IsDetectingATarget())
            {
                entity.StateMachine.SetState<FollowState>();
            }
        }
    }
}