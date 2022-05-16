using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HealthSystem
{
    [System.Serializable]
    public class Shield
    {
        #region Private Fields
        [SerializeField] private float _shieldAmount;
        [SerializeField, Header("Defines if this shield can be damaged")] private bool _canBreak = true;
        public UnityAction OnTakeDamage;
        #endregion

        #region Constructors

        public  Shield() { }

        public Shield(float shieldAmount)
        {
            _shieldAmount = shieldAmount;
        }

        #endregion
        
        #region Public fields
        public float ShieldAmount => _shieldAmount;

        public bool IsActive = true;

        #endregion

        #region Methods
        public void DamageShield(DamageInfo info)
        {
            if(!_canBreak)
                return;

            _shieldAmount -= info.Damage;
            
            OnTakeDamage?.Invoke();
        }

        public void SetAmount(int value) => _shieldAmount = value;

        public void AddAmount(int value) => _shieldAmount += value;

        #endregion
    }
}