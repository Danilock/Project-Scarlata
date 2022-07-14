using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AI {
    public class StoppedState : State<AIAgent>
    {
        public override void OnEnter(AIAgent entity)
        {
            entity.StopCooldownHandler();
            entity.NavAgent.ForceStop();
        }

        public override void OnExit(AIAgent entity)
        {
            entity.CanMove = true;
        }
    }
}