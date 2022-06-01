using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.Rooms
{
    public class RoomManager : PersistentSingleton<RoomManager>
    {
        public Room CurrentActiveRoom;

        private void Start()
        {
            CurrentActiveRoom.EnableRoom();
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
    }
}