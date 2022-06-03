using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Rewriters.Player;

namespace Rewriters.Items
{
    public class ControllableBubble : Pickeable
    {
        [SerializeField, ReadOnly] private Vector2 _initialPosition;
        [SerializeField, ReadOnly] private ControllableBubbleStates _currentState = ControllableBubbleStates.Idle;

        protected override void Awake()
        {
            base.Awake();

            _initialPosition = transform.position;
        }

        protected override void OnPick(GameObject owner)
        {
            owner.transform.SetParent(this.transform);
        }
    }

    public enum ControllableBubbleStates
    {
        OnUse,
        Returning,
        Idle
    }
}