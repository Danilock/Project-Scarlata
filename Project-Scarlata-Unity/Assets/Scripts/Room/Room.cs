using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

namespace Rewriters.Rooms
{
    public class Room : MonoBehaviour
    {
        public CinemachineVirtualCamera Camera;
        public Collider2D Door;

        public UnityAction OnDesactivate, OnActivate;

        [SerializeField] private Color _color;

        public void EnableRoom()
        {
            OnActivate?.Invoke();
            Camera.gameObject.SetActive(true);
        }

        public void DisableRoom()
        {
            OnDesactivate?.Invoke();
            Camera.gameObject.SetActive(false);
        }
    }
}