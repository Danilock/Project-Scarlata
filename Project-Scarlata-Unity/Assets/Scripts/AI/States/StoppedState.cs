using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AI {
    public class StoppedState : State<TargetDetection>
    {
        public override void OnEnter(TargetDetection entity)
        {
            entity.StopCooldownHandler();
            entity.NavAgent.ForceStop();
        }

        public override void OnExit(TargetDetection entity)
        {
            entity.CanMove = true;
        }
    }
}