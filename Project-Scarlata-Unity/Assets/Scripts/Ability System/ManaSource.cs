using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AbilitySystem
{
    public class ManaSource : MonoBehaviour
    {
        [SerializeField] protected float _amount;

        public float GetAmount => _amount;

        [SerializeField] protected float _maxAmount;
    }
}