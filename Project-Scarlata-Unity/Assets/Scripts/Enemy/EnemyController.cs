using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.HealthSystem;
using PathBerserker2d;

namespace Rewriters.Enemies
{
    [RequireComponent(typeof(NavAgent), typeof(EnemyHealth))]
    public class EnemyController : MonoBehaviour
    {
        public NavAgent NavAgent;
        public EnemyHealth Health;
    }
}