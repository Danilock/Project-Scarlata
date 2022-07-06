using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.HealthSystem;

namespace Rewriters.Enemies
{
    public class EnemyHealth : HealthComponent
    {
        [SerializeField] protected bool StunOnGettingDamage;
        [SerializeField] protected EnemyController EnemyController;

        protected override void Start()
        {
            base.Start();

            EnemyController = GetComponent<EnemyController>();
        }
    }
}