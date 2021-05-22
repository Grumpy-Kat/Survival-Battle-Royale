using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrimsonPlague.Managers;

namespace CrimsonPlague.Network {
    public class NetworkWait : Photon.MonoBehaviour {
        public static NetworkWait Instance;

        [Header("Story")]
        [SerializeField]
        Text story;
        [SerializeField]
        List<string> stories;

        [Space(10)]

        [Header("Player Listings")]
        [SerializeField]
        GameObject playerListingPrefab;
        [SerializeField]
        GameObject playerListingParent;
        GameObject[] playerListings = new GameObject[0];
        PhotonPlayer[] playerList = new PhotonPlayer[0];

        [Space(10)]

        [Header("Room and Game Info")]
        [SerializeField]
        Button startBtn;
        [SerializeField]
        int minPlayers;
        [SerializeField]
        Text playerNum;
        [SerializeField]
        Text roomName;

        [Space(10)]

        [Header("Character Customizer")]
        [SerializeField]
        SkinToneManager skinTonePicker;
        [SerializeField]
        OutfitManager outfitPicker;

        void Awake() {
            Instance = this;
            story.text = stories[Random.Range(0, stories.Count)];
            roomName.text = PhotonNetwork.room.Name;
        }

        void Update() {
            if (playerList != PhotonNetwork.playerList) {
                playerList = PhotonNetwork.playerList;
                foreach (GameObject playerListing in playerListings) {
                    Destroy(playerListing);
                }
                playerListings = new GameObject[playerList.Length];
                for (int i = 0; i < playerList.Length; i++) {
                    GameObject obj = Instantiate(playerListingPrefab);
                    obj.transform.SetParent(playerListingParent.transform, false);
                    obj.GetComponent<Text>().text = playerList[i].NickName;
                    playerListings[i] = obj;
                }
                playerNum.text = playerList.Length + " / " + PhotonNetwork.room.MaxPlayers + " Players";
                if (playerList.Length >= minPlayers) {
                    startBtn.interactable = true;
                } else {
                    startBtn.interactable = false;
                }
            }
        }

        public void StartGame() {
            photonView.RPC("StartGameRPC", PhotonTargets.AllViaServer);
        }

        [PunRPC]
        void StartGameRPC() {
            ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.player.CustomProperties;
            properties["skinTone"] = skinTonePicker.currSelectedIndex;
            properties["outfit"] = outfitPicker.currSelectedIndex;
            PhotonNetwork.SetPlayerCustomProperties(properties);
            Invoke("LoadLevel", 0.1f);
        }

        void LoadLevel() {
            GameObject.Find("Network").GetComponent<Network>().Loading();
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.LoadLevel(1);
        }
    }
}