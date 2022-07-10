using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AI
{
    public class IdleState : State<TargetDetection>
    {
        public override void OnUpdate(TargetDetection entity)
        {
            if (entity.IsDetectingATarget())
            {
                entity.StateMachine.SetState<FollowState>();
            }
        }
    }
}