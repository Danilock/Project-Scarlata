using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HealthSystem.Attacks
{
    public class MeleeAttack : Attack
    {
        [SerializeField] private Vector2 DamageAreaSize;
        
        #region Attack Point Variables
        private enum FocusOn
        {
            Offset,
            Target
        }
        [SerializeField] private FocusOn _focusOn;

        private Vector2 _determinePoint
        {
            get
            {
                if (_focusOn == FocusOn.Offset)
                    return (Vector2)transform.position + _offset;
                else
                {
                    return Point.position;
                }
            }
        }
        [SerializeField] private Vector2 _offset;
        #endregion
        [SerializeField] private Color _gizmoColor;
        
        /// <summary>
        /// Does an attack in a square area.
        /// </summary>
        public override void DoAttack()
        {
            //Generates a collider looking for objects
            Collider2D[] damageableHit = Physics2D.OverlapBoxAll(_determinePoint, DamageAreaSize, 0f, Layers);
            
            //Going through all damageables detected
            foreach (Collider2D damageable in damageableHit)
            {
                HealthComponent dmg = damageable.GetComponent<HealthComponent>();
                
                if(dmg == null)
                    return;
                
                dmg.DoDamage(new DamageInfo(Owner, DamageAmount, IgnoreTargetInvulnerability));
                
                if(!dmg.Invulnerable && !dmg.IsDead)
                    OnHit?.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            _gizmoColor.a = 1f;
            
            Gizmos.color = _gizmoColor;
            
            Gizmos.DrawWireCube(_determinePoint, DamageAreaSize);
        }
    }
}