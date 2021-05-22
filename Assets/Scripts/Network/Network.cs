using UnityEngine;
using UnityEngine.UI;

namespace CrimsonPlague.Network {
    public class Network : MonoBehaviour {
        public string version = "0.1";

        public GameObject startMenu;
        public GameObject roomMenu;
        public GameObject waitMenu;
        public GameObject disconnectedMenu;
        public GameObject loadingMenu;

        public InputField userNameField;

        public void Connect() {
            PhotonNetwork.ConnectUsingSettings(version);
        }

        void OnConnectedToMaster() {
            PhotonNetwork.automaticallySyncScene = false;
            if (userNameField.text == "") {
                PhotonNetwork.playerName = PlayerNetwork.Instance.playerName;
            } else {
                PhotonNetwork.playerName = userNameField.text;
            }
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }

        void OnJoinedLobby() {
            startMenu.SetActive(false);
            roomMenu.SetActive(true);
            waitMenu.SetActive(false);
            disconnectedMenu.SetActive(false);
            loadingMenu.SetActive(false);
        }

        void OnDisconnectedFromPhoton() {
            startMenu.SetActive(false);
            roomMenu.SetActive(false);
            waitMenu.SetActive(false);
            disconnectedMenu.SetActive(true);
            loadingMenu.SetActive(false);
        }

        public void Waiting() {
            startMenu.SetActive(false);
            roomMenu.SetActive(false);
            waitMenu.SetActive(true);
            disconnectedMenu.SetActive(false);
            loadingMenu.SetActive(false);
        }

        public void Loading() {
            startMenu.SetActive(false);
            roomMenu.SetActive(false);
            //waitMenu.SetActive(false);
            disconnectedMenu.SetActive(false);
            loadingMenu.SetActive(true);
        }
    }
}