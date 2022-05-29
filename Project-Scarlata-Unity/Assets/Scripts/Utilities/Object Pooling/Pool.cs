using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ObjectPooling
{
    [Serializable, InlineProperty(LabelWidth = 90)]
    public class Pool
    {
        private int _currentIndex = 0;

        #region Public Fields
        
        [FoldoutGroup("Pool Settings"), AssetsOnly] public GameObject ObjectToInstantiate;
        [FoldoutGroup("Pool Settings")] public int Amount;
        [FoldoutGroup("Pool Settings"), ReadOnly] public List<GameObject> Instances = new List<GameObject>();

        #endregion

        /// <summary>
        /// Returns an object from this pool by activating it.
        /// </summary>
        /// <returns></returns>
        public GameObject GetObject()
        {
            GameObject objectObtainedInPool = Instances[_currentIndex];
            
            objectObtainedInPool.SetActive(true);

            _currentIndex = (_currentIndex + 1) % Instances.Count;

            return objectObtainedInPool;
        }
    }
}