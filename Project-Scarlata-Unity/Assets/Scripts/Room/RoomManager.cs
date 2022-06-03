using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Rewriters.Rooms
{
    public class RoomManager : PersistentSingleton<RoomManager>
    {
        [SerializeField] private List<Room> _allRoomsInScene;
        public Room CurrentActiveRoom;

        private void Start()
        {
            DisableAllRooms();

            ActivateRoom(_allRoomsInScene[0]);
        }

        public void ActivateRoom(Room roomToActivate)
        {
            StartCoroutine(HandleRoomChange_CO(roomToActivate));
        }

        private IEnumerator HandleRoomChange_CO(Room roomToActivate)
        {
            Time.timeScale = 0f;

            if (CurrentActiveRoom != null)
                CurrentActiveRoom.DisableRoom();

            CurrentActiveRoom = roomToActivate;

            CurrentActiveRoom.EnableRoom();

            yield return new WaitForSecondsRealtime(.3f);

            Time.timeScale = 1f;
        }

        private void DisableAllRooms()
        {
            foreach(Room currentRoom in _allRoomsInScene)
            {
                currentRoom.DisableRoom();
            }
        }

        [Button("Find All Rooms")]
        public void FindAllRooms()
        {
            _allRoomsInScene = FindObjectsOfType<Room>().ToList();
        }
    }
}