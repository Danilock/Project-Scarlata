using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.AI;

namespace Rewriters.Enemies
{
    public class RedCubeController : EnemyController
    {
        public StateMachine<RedCubeController> StateMachine;
        public TargetDetection TargetDetection;

        protected override void Awake()
        {
            base.Awake();

            TargetDetection = GetComponent<TargetDetection>();

            StateMachine = new StateMachine<RedCubeController>(this);

            StateMachine.AddState<RedCubeIdleState>();
            StateMachine.AddState<RedCubeFollowState>();

            StateMachine.SetState<RedCubeIdleState>();
        }

        private void Update()
        {
            StateMachine.CurrentState.OnUpdate(this);
        }
    }
}