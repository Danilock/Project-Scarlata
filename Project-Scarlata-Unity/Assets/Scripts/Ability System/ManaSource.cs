using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AbilitySystem
{
    public class ManaSource : MonoBehaviour
    {
        [SerializeField] protected float _currentAmount;

        public float GetCurrentAmount => _currentAmount;

        [SerializeField] protected float _maxAmount;

        public void AddMana(float amount)
        {
            if(amount + _currentAmount > _maxAmount)
            {
                _currentAmount = _maxAmount;
                return;
            }

            _currentAmount += amount;
        }
    }
}