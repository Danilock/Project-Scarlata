using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HealthSystem
{
    /// <summary>
    /// Contains information about a damage Transmitter.
    /// </summary>
    public class DamageInfo
    {
        /// <summary>
        /// Who is causing this damage.
        /// </summary>
        public HealthComponent Transmitter;

        public float Damage;
        public bool IgnoreInvulnerability = false;

        public DamageInfo()
        {
        }

        public DamageInfo(HealthComponent transmitter, float damage, bool ignoreInvulnerability)
        {
            this.Transmitter = transmitter;
            this.Damage = damage;
            this.IgnoreInvulnerability = ignoreInvulnerability;
        }
    }
}