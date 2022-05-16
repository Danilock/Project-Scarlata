using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace EventDriven
{
    public static class EventManager
    {
        /// <summary>
        /// Dictionary of all registered events.
        /// </summary>
        private static Dictionary<Type, List<object>> EventsRegister = new Dictionary<Type, List<object>>();

        /// <summary>
        /// Triggers the given event.
        /// </summary>
        /// <param name="eventToTrigger"></param>
        /// <typeparam name="T"></typeparam>
        public static void TriggerEvent<T>(T eventToTrigger) where T : struct
        {
            if (EventsRegister.ContainsKey(typeof(T)))
            {
                foreach (var listener in EventsRegister[typeof(T)])
                {
                    var currentListener = (IEventListener<T>) listener;
                    currentListener.OnTriggerEvent(eventToTrigger);
                }
            }
        }

        /// <summary>
        /// Adds a listener to the event.
        /// </summary>
        /// <param name="eventToListen">Specify the event to listen.</param>
        /// <param name="listener">Specify the object that will be listening</param>
        /// <typeparam name="T"></typeparam>
        public static void AddListener<T>(T eventToListen, IEventListener<T> listener) where T : struct
        {
            if (EventsRegister.ContainsKey(typeof(T)))
            {
                EventsRegister[typeof(T)].Add(listener);
                return;
            }

            EventsRegister.Add(typeof(T), new List<object> {listener});
        }

        /// <summary>
        /// Removes a listener for the given event.
        /// </summary>
        /// <param name="eventToListen"></param>
        /// <param name="listener"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveListener<T>(T eventToListen, IEventListener<T> listener) where T : struct
        {
            if (EventsRegister.ContainsKey(typeof(T)))
                EventsRegister[typeof(T)].Remove(listener);
        }
    }
}