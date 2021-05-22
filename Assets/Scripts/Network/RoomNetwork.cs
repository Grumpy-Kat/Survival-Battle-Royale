using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrimsonPlague.Network {
    public class RoomNetwork : MonoBehaviour {
        public static RoomNetwork Instance;

        [Header("Create Room")]
        [SerializeField]
        Text roomName;
        [SerializeField]
        InputField inputField;

        [Space(10)]

        [Header("Room Listings")]
        [SerializeField]
        GameObject roomListingPrefab;
        [SerializeField]
        GameObject roomListingParent;
        List<RoomListing> roomListings = new List<RoomListing>();

        [Space(10)]

        [Header("Join Room")]
        [SerializeField]
        ScrollRect scroll;
        int currSelectedIndex = -1;

        void Awake() {
            Instance = this;
        }

        void Update() {
            if (!inputField.isFocused && Input.GetButtonDown("Inventory")) {
                if (roomName.text == "") {
                    string[] text = Resources.Load<TextAsset>("Data/RoomNames").text.Split('\n');
                    inputField.text = text[UnityEngine.Random.Range(0, text.Length - 1)] + " " + UnityEngine.Random.Range(0, 1000).ToString();
                }
                CreateRoom();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                currSelectedIndex--;
                if (currSelectedIndex < -1) {
                    currSelectedIndex = roomListings.Count - 1;
                } else if (currSelectedIndex > roomListings.Count - 1) {
                    currSelectedIndex = -1;
                }
                UpdatePicker();
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                currSelectedIndex++;
                if (currSelectedIndex < -1) {
                    currSelectedIndex = roomListings.Count - 1;
                } else if (currSelectedIndex > roomListings.Count - 1) {
                    currSelectedIndex = -1;
                }
                UpdatePicker();
            }
            if (!inputField.isFocused && currSelectedIndex != -1 && Input.GetButtonDown("Drop")) {
                RoomNetwork.Instance.JoinRoom(roomListings[currSelectedIndex].roomName.text);
            }
        }

        void UpdatePicker() {
            roomListings[currSelectedIndex].roomName.GetComponentInParent<Button>().interactable = false;
            scroll.verticalScrollbar.value = 1 - ((float)currSelectedIndex / (roomListings.Count - 1));
        }

        void OnReceivedRoomListUpdate() {
            RoomInfo[] rooms = PhotonNetwork.GetRoomList();
            foreach (RoomInfo room in rooms) {
                int i = roomListings.FindIndex(x => x.roomName.text == room.Name);
                if (i == -1) {
                    if (room.IsVisible && room.PlayerCount < room.MaxPlayers) {
                        GameObject obj = Instantiate(roomListingPrefab);
                        obj.transform.SetParent(roomListingParent.transform, false);
                        roomListings.Add(obj.GetComponent<RoomListing>());
                        i = roomListings.Count - 1;
                    }
                }
                if (i != -1) {
                    RoomListing roomListing = roomListings[i];
                    roomListing.SetRoomName(room.Name);
                    roomListing.updated = true;
                }
            }
            List<RoomListing> newRoomListings = new List<RoomListing>();
            newRoomListings.AddRange(roomListings);
            foreach (RoomListing roomListing in roomListings) {
                if (!roomListing.updated) {
                    GameObject obj = roomListing.gameObject;
                    newRoomListings.Remove(roomListing);
                    Destroy(obj);
                } else {
                    roomListing.updated = false;
                }
            }
            roomListings.Clear();
            roomListings.AddRange(newRoomListings);
        }

        public void CreateRoom() {
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            properties.Add("s", (int)DateTime.Now.Ticks);
            if (!PhotonNetwork.CreateRoom(roomName.text, new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 20, CustomRoomProperties = properties }, TypedLobby.Default)) {
                Debug.Log("Unable to create room " + roomName.text);
            }
        }

        void OnPhotonCreateRoomFailed(object[] e) {
            Debug.Log(e[1]);
        }

        void OnCreatedRoom() {
            Debug.Log("Created room");
        }

        public void JoinRoom(string roomName) {
            if (!PhotonNetwork.JoinRoom(roomName)) {
                Debug.Log("Unable to join room " + roomName);
            }
        }

        void OnJoinedRoom() {
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            properties.Add("alive", true);
            properties.Add("ready", false);
            properties.Add("skinTone", 0);
            properties.Add("outfit", 0);
            PhotonNetwork.SetPlayerCustomProperties(properties);
            GameObject.Find("Network").GetComponent<Network>().Waiting();
        }
    }
}