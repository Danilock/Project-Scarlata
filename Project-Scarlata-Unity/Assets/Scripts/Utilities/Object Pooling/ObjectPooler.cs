using System;
using System.Collections;
using System.Collections.Generic;
using ObjectPooling;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;

namespace ObjectPooling
{
    [DictionaryDrawerSettings(KeyLabel = "Pool Key", ValueLabel = "Pool")]
    public class ObjectPooler : PersistantSerializedSingleton<ObjectPooler>
    {
        [SerializeField] private Dictionary<string, Pool> Pools = new Dictionary<string, Pool>();

        protected override void Awake()
        {
            base.Awake();

            InitializePools();
        }

        private void InitializePools()
        {
            GameObject obj = new GameObject("Pool System");

            obj.transform.SetParent(this.transform);

            foreach (Pool currentPool in Pools.Values)
            {
                GameObject parent = new GameObject($"{currentPool.ObjectToInstantiate.name} () Pool Objects");
            
                for (int i = 0; i < currentPool.Amount; i++)
                {
                    GameObject instanceCreated = Instantiate(currentPool.ObjectToInstantiate, parent.transform.position, Quaternion.identity);
                    instanceCreated.SetActive(false);
                    instanceCreated.transform.SetParent(parent.transform);
                
                    currentPool.Instances.Add(instanceCreated);
                }

                parent.transform.SetParent(obj.transform);
            }
        }

        /// <summary>
        /// Gets an object from the specified pool.
        /// </summary>
        /// <param name="poolKey"></param>
        /// <returns></returns>
        public GameObject GetObjectFromPool(string poolKey)
        {
            return Pools[poolKey].GetObject();
        }
    }
}