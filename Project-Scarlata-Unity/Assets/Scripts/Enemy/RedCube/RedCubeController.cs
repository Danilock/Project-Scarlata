using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.Enemies
{
    public class RedCubeController : EnemyController
    {
        public StateMachine<RedCubeController> StateMachine;

        private void Awake()
        {
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