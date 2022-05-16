using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventDriven
{
    public interface IEventListener<T>
    {
        public void OnTriggerEvent(T data);
    }
}