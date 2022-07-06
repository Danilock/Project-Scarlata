using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.Enemies
{
    public class RedCubeIdleState : State<RedCubeController>
    {
        public override void OnEnter(RedCubeController entity)
        {
            Debug.Log("State Initialized");
        }

        public override void OnUpdate(RedCubeController entity)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                entity.StateMachine.SetState<RedCubeFollowState>();
            }
        }
    }
}