using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.HealthSystem;
using PathBerserker2d;
using Rewriters.AI;

namespace Rewriters.Enemies
{
    [RequireComponent(typeof(TargetDetection), typeof(EnemyHealth))]
    public class EnemyController : MonoBehaviour
    {
        public TargetDetection TargetDetection;
        public EnemyHealth Health;

        protected virtual void Awake()
        {
            TargetDetection = GetComponent<TargetDetection>();
            Health = GetComponent<EnemyHealth>();
        }
    }
}