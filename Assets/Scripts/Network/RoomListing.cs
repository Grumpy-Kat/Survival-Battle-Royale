using UnityEngine;
using UnityEngine.UI;

namespace CrimsonPlague.Network {
    public class RoomListing : MonoBehaviour {
        public Text roomName { get; protected set; }
        public bool updated = false;

        void Start() {
            roomName = GetComponentInChildren<Text>();
            Button btn = GetComponent<Button>();
            btn.onClick.AddListener(() => RoomNetwork.Instance.JoinRoom(roomName.text));
        }

        void OnDestroy() {
            Button btn = GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
        }

        public void SetRoomName(string text) {
            if (roomName == null) {
                roomName = GetComponentInChildren<Text>();
            }
            roomName.text = text;
        }
    }
}