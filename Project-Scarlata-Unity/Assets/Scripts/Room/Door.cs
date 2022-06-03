using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.Rooms
{   
    /// <summary>
    /// In game representation of a Door which notifies the roomManager once is triggered.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class Door : MonoBehaviour
    {
        [SerializeField] private Room _roomToEnable;
        private BoxCollider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
        }

        private void OnEnable()
        {
            _roomToEnable.OnActivate += DesactivateDoor;
            _roomToEnable.OnDesactivate += ActivateDoor;
        }

        private void OnDisable()
        {
            _roomToEnable.OnActivate -= DesactivateDoor;
            _roomToEnable.OnDesactivate -= ActivateDoor;
        }

        public void DesactivateDoor() => _collider.enabled = false;

        public void ActivateDoor() => _collider.enabled = true;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
                return;

            RoomManager.Instance.ActivateRoom(_roomToEnable);
        }
    }
}
