using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

namespace HealthSystem
{
    /// <summary>
    /// Representation of a damageable entity in game.
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        #region Protected/Private Fields
        //Health
        [SerializeField] protected float _startHealth = 1;
        [SerializeField] protected float _currentHealth = 1;

        private Color GetColorBar
        {
            get
            {
                return this._currentHealth >  _startHealth * .7f ? Color.green :
                    this._currentHealth > _startHealth * .3f ? Color.yellow : Color.red;
            }
        }
        
        public Shield Shield = new Shield();
        [SerializeField] private bool _invulnerable = false;

        public bool IsDead
        {
            get;
            protected set;
        }
        #endregion

        #region Public Fields

        public float StartHealth => _startHealth;

        public float CurrentHealth => _currentHealth;

        public bool Invulnerable
        {
            get => _invulnerable;
            protected set => _invulnerable = value;
        }

        #endregion
        
        #region Events

        public UnityAction<DamageInfo> OnTakeDamage, OnDeath;

        #endregion

        #region Methods

        protected virtual void Start()
        {
            _currentHealth = _startHealth;
        }

        private void OnValidate()
        {
            if (_currentHealth > _startHealth)
                _currentHealth = _startHealth;
        }

        /// <summary>
        /// Do damage to this entity by receiving a damage info struct.
        /// </summary>
        /// <param name="incomingDamage"></param>
        public virtual void DoDamage(DamageInfo incomingDamage)
        {
            if (IsDead)
                return;

            if(_invulnerable && !incomingDamage.IgnoreInvulnerability)
                return;

            if (Shield.ShieldAmount > 0)
            {
                Shield.DamageShield(incomingDamage);
                return;
            }

            _currentHealth -= incomingDamage.Damage;

            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke(incomingDamage);
                IsDead = true;
                _currentHealth = 0;
            }
            else
                OnTakeDamage?.Invoke(incomingDamage);
        }

        /// <summary>
        /// Sets the health immediately to the specified value.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetHealth(int value)
        {
            if (value > _startHealth)
                _startHealth = value;

            _currentHealth = value;
        }

        #region Invulnerability
        public virtual void SetInvulnerableForSeconds(float seconds) => StartCoroutine(SetInvulnerableForSeconds_CO(seconds));

        protected virtual IEnumerator SetInvulnerableForSeconds_CO(float seconds)
        {
            Invulnerable = true;
            yield return new WaitForSeconds(seconds);
            Invulnerable = false;
        }

        public virtual void SetInvulnerable(bool value) => Invulnerable = value;

        #endregion

        #endregion
    }
}