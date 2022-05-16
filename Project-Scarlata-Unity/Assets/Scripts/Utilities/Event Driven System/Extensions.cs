using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventDriven
{
    public static class Extensions
    {
        public static void StartListening<T>(this IEventListener<T> listener) where T : struct
        {
            EventManager.AddListener(new T(), listener);
        }

        public static void StopListening<T>(this IEventListener<T> listener) where T : struct
        {
            EventManager.RemoveListener(new T(), listener);
        }
    }
}