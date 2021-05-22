using UnityEngine;
using UnityEngine.SceneManagement;
using CrimsonPlague.Managers;

namespace CrimsonPlague.Network {
    public class PlayerNetwork : MonoBehaviour {
        public static PlayerNetwork Instance;
        public string playerName { get; protected set; }

        public GameObject playerPrefab;
        public GameObject inventoryPrefab;

        public bool isSinglePlayer;

        GameObject playerObj;
        GameObject inventoryObj;

        void Awake() {
            DontDestroyOnLoad(this);
            Instance = this;
            playerName = "Player";
            SceneManager.sceneLoaded += OnSceneFinishedLoading;
        }

        void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
            if (scene.name == "Game") {
                float x = Random.Range(-485, 485);
                float z = Random.Range(-485, 485);
                //float x = 0;
                //float z = 0;
                float y = 200;
                playerObj = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(x, y, z), playerPrefab.transform.rotation, 0);
                inventoryObj = PhotonNetwork.Instantiate(inventoryPrefab.name, Vector3.zero, Quaternion.identity, 0);
                Player.Player playerComp = playerObj.GetComponentInChildren<Player.Player>();
                playerComp.transform.GetChild(playerComp.transform.childCount - 1).GetComponent<MeshRenderer>().material.color = Color.blue;
                Camera[] cams = playerComp.transform.GetComponentsInChildren<Camera>(true);
                foreach (Camera cam in cams) {
                    cam.gameObject.SetActive(true);
                }
                InventoryManager inventoryComp = inventoryObj.GetComponent<InventoryManager>();
                inventoryComp.player = playerComp;
                playerComp.inventory = inventoryComp;
                MapManager.Instance.player = playerComp;
            }
        }

        public void Die(float time) {
            PhotonNetwork.Destroy(playerObj);
            PhotonNetwork.Destroy(inventoryObj);
            ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.player.CustomProperties;
            properties["alive"] = true;
            PhotonNetwork.SetPlayerCustomProperties(properties);
            int playersAlive = 0;
            foreach (PhotonPlayer player in PhotonNetwork.playerList) {
                if ((bool)player.CustomProperties["alive"]) {
                    playersAlive++;
                }
            }
            switch (playersAlive) {
                case 0:
                    //1st Place, recieve 10 coins
                    Debug.Log("1st Place");
                    break;
                case 1:
                    //2nd Place, recieve 7 coins
                    Debug.Log("2nd Place");
                    break;
                case 2:
                    //3rd Place, recieve 5 coins
                    Debug.Log("3rd Place");
                    break;
                default:
                    //Other, recieve 3 coins
                    Debug.Log("Didn't Place");
                    break;
            }
            PhotonNetwork.LeaveRoom();
            Invoke("ReturnToMainMenu", time);
        }

        void ReturnToMainMenu() {
            PhotonNetwork.LoadLevel("MainMenu");
        }
    }
}