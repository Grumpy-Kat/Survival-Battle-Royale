using UnityEngine;
using UnityEngine.UI;

namespace CrimsonPlague.Managers {
    public class MapManager : MonoBehaviour {
        public static MapManager Instance;

        public GameObject map { get; protected set; }
        GameObject miniMap;
        Button openMapBtn;
        Button closeMapBtn;
        Transform cameraObj;
        Camera cam;

        [SerializeField]
        float posSpeed = 5f;
        [SerializeField]
        float scrollSpeed = 1f;

        [SerializeField]
        Vector3 defaultPos = new Vector3(0f, 15f, 0f);
        [SerializeField]
        float defaultScroll = 10f;

        [SerializeField]
        float minScroll = 3f;
        [SerializeField]
        float maxScroll = 100f;

        [HideInInspector]
        public Player.Player player;

        [HideInInspector]
        public bool isUseable = false;

        void Awake() {
            if (Instance == null) {
                Instance = this;
            }

            map = GameObject.Find("Canvas").transform.Find("FullScreenMap").gameObject;
            miniMap = GameObject.Find("Canvas").transform.Find("Map").gameObject;
            closeMapBtn = map.transform.Find("Close").GetComponent<Button>();
            openMapBtn = GameObject.Find("Canvas").transform.Find("OpenMapButton").GetComponent<Button>();
        }

        void Update() {
            // Controls overhead camera for mini map
            if (!isUseable) {
                map.SetActive(false);
                openMapBtn.gameObject.SetActive(false);
                miniMap.SetActive(false);
            }
            
            if (cameraObj == null && player != null) {
                cameraObj = player.transform.Find("MapCamera");
                cam = cameraObj.GetComponent<Camera>();
            }

            if (cameraObj != null && isUseable) {
                if (player.ready && !player.inventory.panel.activeInHierarchy) {
                    if (Input.GetButtonDown("Map")) {
                        if (openMapBtn != null) {
                            if (openMapBtn.IsActive()) {
                                openMapBtn.onClick.Invoke();
                            } else {
                                if (closeMapBtn != null) {
                                    closeMapBtn.onClick.Invoke();
                                    cameraObj.localPosition = defaultPos;
                                    cam.orthographicSize = defaultScroll;
                                }
                            }
                        }
                    }
                }

                if (map.activeInHierarchy) {
                    cam.orthographicSize -= cam.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
                    cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minScroll, maxScroll);
                    cameraObj.localPosition = cameraObj.localPosition + new Vector3(Input.GetAxis("Horizontal") * posSpeed * (cam.orthographicSize / 10), 0, Input.GetAxis("Vertical") * posSpeed * (cam.orthographicSize / 10));
                    float height = cam.orthographicSize * 2;
                    float width = height * Screen.width / Screen.height;
                    Vector3 pos = cameraObj.position;
                    float xChange = 0f;
                    float zChange = 0f;
                    if (pos.x < -500 + width / 2) {
                        xChange = pos.x - (-500 + width / 2);
                    } else if (pos.x > 500 - width / 2) {
                        xChange = pos.x - (500 - width / 2);
                    }
                    if (pos.z < -500 + height / 2) {
                        zChange = pos.z - (-500 + height / 2);
                    } else if (pos.z > 500 - height / 2) {
                        zChange = pos.z - (500 - height / 2);
                    }
                    cameraObj.localPosition = cameraObj.localPosition + new Vector3(-xChange, 0, -zChange);
                }
            }
        }
    }
}