using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using CrimsonPlague.Managers;
using CrimsonPlague.Network;
using CrimsonPlague.Items;

namespace CrimsonPlague.Player {
    public class Player : Photon.MonoBehaviour {
        [Header("Various Objects")]
        public PlayerMove playerMove;
        //[HideInInspector]
        public InventoryManager inventory;

        GameObject loading;

        public GameObject bleedPanel;

        public GameObject waterPanel;
        public float waterLevel = 0;

        public GameObject wonMenu { get; protected set; }
        public GameObject deadMenu { get; protected set; }
        public GameObject deadCam { get; protected set; }
        public float screenOpenTime = 10;

        public Transform hand;

        [SerializeField]
        bool statsDecrease = true;
        public bool useMap = true;
        public bool singlePlayer = false;

        [Space(10)]

        [Header("Health")]
        [SerializeField]
        float health = 100f;
        [SerializeField]
        float healthRegenRate = 6f;
        float currHealthRegenTime = 0;
        RectTransform healthBar;
        [SerializeField]
        GameObject bloodParticle;
        List<Coroutine> bleeding;
        List<GameObject> bleedingObj;

        [Space(10)]

        [Header("Hunger")]
        [SerializeField]
        float hunger = 100f;
        [SerializeField]
        float minHunger = 10f;
        [SerializeField]
        float hungerRate = 4.2f;
        float currHungerTime = 0;
        RectTransform hungerBar;


        [Space(10)]

        [Header("Thirst")]
        [SerializeField]
        float thirst = 100f;
        [SerializeField]
        float minThirst = 10f;
        [SerializeField]
        float thirstRate = 3;
        float currThirstTime = 0;
        RectTransform thirstBar;


        [Space(10)]

        [Header("Warmth")]
        [SerializeField]
        float warmth = 50f;
        [SerializeField]
        float warmthRate = 10;
        float currWarmthTime = 0;
        float tempModifier;
        RectTransform warmthBar;


        [Space(10)]

        [Header("Energy")]
        [SerializeField]
        float stamina = 100f;
        [SerializeField]
        float staminaRegenRate = 1f;
        float currStaminaRegenTime = 0;
        RectTransform staminaBar;
        public bool canRun { get; protected set; }
        public bool canJump { get; protected set; }

        float barMultiplier = 1;

        public bool ready { get; protected set; }

        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        AudioClip[] damageAudio;
        [SerializeField]
        AudioClip[] continuousDamageAudio;
        AudioSource src;

        void Awake() {
            ready = false;
            hand = GetChild(transform, "rHand");
            bleeding = new List<Coroutine>();
            bleedingObj = new List<GameObject>();
            if (CustomizeManager.Instance != null) {
                CustomizeManager.Instance.WearSkinTone(int.Parse(photonView.owner.CustomProperties["skinTone"].ToString()), gameObject);
                CustomizeManager.Instance.WearOutfit(int.Parse(photonView.owner.CustomProperties["outfit"].ToString()), gameObject);
            }
            src = gameObject.GetComponent<AudioSource>();
            if (photonView.isMine || singlePlayer) {
                playerMove = gameObject.GetComponent<PlayerMove>();
                if (statsDecrease) {
                    bleedPanel = GameObject.Find("Canvas").transform.Find("BleedPanel").gameObject;
                    waterPanel = GameObject.Find("Canvas").transform.Find("WaterPanel").gameObject;
                }
                if (PhotonNetwork.room != null) {
                    CallGenerateTerrain((int)PhotonNetwork.room.CustomProperties["s"]);
                }
                List<Player> players = new List<Player>();
                if (!singlePlayer) {
                    loading = GameObject.Find("Canvas").transform.Find("Loading").gameObject;
                    loading.SetActive(true);
                }
                if (statsDecrease) {
                    deadCam = GameObject.Find("Overhead");
                    if (deadCam == null) {
                        if (players.Count == 0) {
                            players = GameObject.FindObjectsOfType<Player>().ToList();
                        }
                        foreach (Player player in players) {
                            if (player.deadMenu != null) {
                                deadCam = player.deadCam;
                                break;
                            }
                        }
                    }
                    if (deadCam != null) {
                        deadCam.SetActive(false);
                    }
                    deadMenu = GameObject.Find("Canvas").transform.Find("DiedMenu").gameObject;
                    if (deadMenu == null) {
                        if (players.Count == 0) {
                            players = GameObject.FindObjectsOfType<Player>().ToList();
                        }
                        foreach (Player player in players) {
                            if (player.deadMenu != null) {
                                deadMenu = player.deadMenu;
                                break;
                            }
                        }
                    }
                    wonMenu = GameObject.Find("Canvas").transform.Find("WonMenu").gameObject;
                    if (wonMenu == null) {
                        if (players.Count == 0) {
                            players = GameObject.FindObjectsOfType<Player>().ToList();
                        }
                        foreach (Player player in players) {
                            if (player.wonMenu != null) {
                                wonMenu = player.wonMenu;
                                break;
                            }
                        }
                    }
                    if (deadMenu != null) {
                        deadMenu.SetActive(false);
                    }
                }
                Transform stats = GameObject.Find("Canvas").transform.Find("Stats");
                healthBar = (RectTransform)(stats.Find("HealthBar").GetChild(0));
                hungerBar = (RectTransform)(stats.Find("HungerBar").GetChild(0));
                thirstBar = (RectTransform)(stats.Find("ThirstBar").GetChild(0));
                warmthBar = (RectTransform)(stats.Find("WarmthBar").GetChild(0));
                staminaBar = (RectTransform)(stats.Find("StaminaBar").GetChild(0));
                barMultiplier = healthBar.rect.width / 100;
            }
            playerMove.Init();
        }

        public void Init() {
            Awake();
        }

        void CallGenerateTerrain(int seed) {
            GenerateTerrain terrain = FindObjectOfType<GenerateTerrain>();
            terrain.seed = seed;
            tempModifier = terrain.Generate();
            waterLevel = terrain.waterLevel;
            GenerateY();
        }

        void GeneratePos() {
            System.Random random = new System.Random();
            float x = random.Next(-485, 485);
            float z = random.Next(-485, 485);
            transform.parent.position = new Vector3(x, 200, z);
            GenerateY();
        }

        void GenerateY() {
            RaycastHit hit;
            Ray ray = new Ray(transform.parent.position, Vector3.down);
            if (Physics.Raycast(ray, out hit)) {
                transform.parent.Translate(new Vector3(0, -(hit.distance - 5), 0));
                if (transform.parent.position.y <= waterLevel + 2) {
                    GeneratePos();
                }
            }
        }

        Transform GetChild(Transform parent, string name) {
            Transform result = parent.Find(name);
            if (result != null) {
                return result;
            }
            for (int i = 0; i < parent.childCount; i++) {
                result = GetChild(parent.GetChild(i), name);
                if (result != null) {
                    return result;
                }
            }
            return null;
        }

        void Update() {
            if (photonView.isMine || singlePlayer) {
                //setting up
                if (!singlePlayer && !ready && ((bool)PhotonNetwork.player.CustomProperties["ready"])) {
                    ready = true;
                    foreach (PhotonPlayer player in PhotonNetwork.playerList) {
                        if (!((bool)player.CustomProperties["ready"])) {
                            ready = false;
                        }
                    }
                    if (ready) {
                        loading.SetActive(false);
                    } else {
                        loading.SetActive(true);
                    }
                } else {
                    Move();
                    if (statsDecrease) {
                        Water();
                    }
                    Drop();
                    PickUp();
                    if (statsDecrease) {
                        HealthRegen();
                        TakeHunger();
                        TakeThirst();
                        TakeWarmth();
                        StaminaRegen();
                        TestDead();
                        TestWon();
                    }
                }
                ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.player.CustomProperties;
                properties["ready"] = true;
                PhotonNetwork.SetPlayerCustomProperties(properties);
            }
        }

        void Move() {
            if (!inventory.panel.activeInHierarchy && (!useMap || !MapManager.Instance.map.activeInHierarchy)) {
                playerMove.Move();
            }
        }

        void Water() {
            if (transform.position.y < waterLevel - 1.5f) {
                waterPanel.SetActive(true);
                foreach (Animator animator in playerMove.animators) {
                    animator.SetBool("Swimming", true);
                }
            } else {
                waterPanel.SetActive(false);
                foreach (Animator animator in playerMove.animators) {
                    animator.SetBool("Swimming", false);
                }
            }
        }

        void Drop() {
            if (!inventory.panel.activeInHierarchy && (!useMap || !MapManager.Instance.map.activeInHierarchy) && Input.GetButtonDown("Drop")) {
                if (inventory.currSelectedItem.main != "Necessity") {
                    inventory.DropItem(inventory.currSelectedItem);
                }
            }
        }

        void PickUp() {
            if (!inventory.panel.activeInHierarchy && (!useMap || !MapManager.Instance.map.activeInHierarchy) && Input.GetButtonDown("PickUp")) {
                Ray ray = playerMove.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                Debug.DrawRay(playerMove.cam.transform.position, ray.direction, Color.magenta);
                if (Physics.Raycast(ray, out hit)) {
                    Item item = hit.transform.GetComponent<Item>();
                    if (item != null) {
                        inventory.AddItem(item);
                    }
                }
            }
        }

        void HealthRegen() {
            if (hunger > minHunger && thirst > minThirst) {
                if (health < 100) {
                    currHealthRegenTime += Time.deltaTime;
                    if (currHealthRegenTime >= healthRegenRate) {
                        health += 1;
                        health = Mathf.Clamp(health, 0, 100);
                        currHealthRegenTime = 0;
                    }
                } else {
                    currHealthRegenTime = 0;
                }
            }
        }

        void TakeHunger() {
            currHungerTime += Time.deltaTime;
            if (currHungerTime >= hungerRate) {
                hunger -= 1;
                if (hunger <= 0) {
                    AddHealth(-1);
                }
                currHungerTime = 0;
                hungerBar.sizeDelta = new Vector2(hunger * barMultiplier, hungerBar.sizeDelta.y);
            }
        }

        void TakeThirst() {
            currThirstTime += Time.deltaTime;
            if (currThirstTime >= thirstRate) {
                thirst -= 1;
                if (thirst <= 0) {
                    AddHealth(-1);
                }
                currThirstTime = 0;
                thirstBar.sizeDelta = new Vector2(thirst * barMultiplier, thirstBar.sizeDelta.y);
            }
        }

        void TakeWarmth() {
            currWarmthTime += Time.deltaTime;
            if (currWarmthTime >= warmthRate) {
                if (transform.position.y <= 25f) {
                    //in water
                    warmth -= ((transform.position.y - 25f) * 0.02f) * tempModifier;
                } else if (transform.position.y >= 150f) {
                    //on mountains
                    warmth -= ((150f - transform.position.y) * 0.02f) * tempModifier;
                } else {
                    warmth -= 0.1f * -tempModifier;
                }
                if (warmth <= 0 || warmth >= 100) {
                    AddHealth(-1);
                }
                currWarmthTime = 0;
                warmthBar.sizeDelta = new Vector2(warmth * barMultiplier, warmthBar.sizeDelta.y);
            }
        }

        void StaminaRegen() {
            if (stamina < 100 && !playerMove.isRunning) {
                currStaminaRegenTime += Time.deltaTime;
                if (currStaminaRegenTime >= staminaRegenRate) {
                    stamina += 1;
                    stamina = Mathf.Clamp(stamina, 0, 100);
                    currStaminaRegenTime = 0;
                }
                staminaBar.sizeDelta = new Vector2(stamina * barMultiplier, staminaBar.sizeDelta.y);
            } else {
                currStaminaRegenTime = 0;
            }
        }

        void TestDead() {
            if (health <= 0) {
                deadCam.SetActive(true);
                deadMenu.SetActive(true);
                deadMenu.transform.GetChild(0).GetComponent<Text>().text = OrdinalForm(PhotonNetwork.playerList.Length) + " Place";
                List<List<Item>> inventoryItems = inventory.inventoryItems.Values.ToList();
                foreach (List<Item> items in inventoryItems) {
                    foreach (Item item in items) {
                        inventory.DropItem(item);
                    }
                }
                PlayerNetwork.Instance.Die(screenOpenTime);
            }
        }

        void TestWon() {
            if (!PlayerNetwork.Instance.isSinglePlayer && PhotonNetwork.playerList.Length == 1) {
                deadCam.SetActive(true);
                wonMenu.SetActive(true);
                PlayerNetwork.Instance.Die(screenOpenTime);

            }
        }

        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.isWriting) {
                stream.SendNext(health);
                stream.SendNext(hunger);
                stream.SendNext(thirst);
                stream.SendNext(warmth);
                stream.SendNext(stamina);
            } else {
                health = (float)stream.ReceiveNext();
                hunger = (float)stream.ReceiveNext();
                thirst = (float)stream.ReceiveNext();
                warmth = (float)stream.ReceiveNext();
                stamina = (float)stream.ReceiveNext();
            }
        }

        public void TakeDamage(float amt, bool bleed = false, Vector3 point = new Vector3(), Quaternion orientation = new Quaternion(), string bodyPart = "All") {
            photonView.RPC("TakeDamageActual", PhotonTargets.All, amt, bleed, point, orientation, bodyPart);
        }

        [PunRPC]
        void TakeDamageActual(float amt, bool bleed, Vector3 point, Quaternion orientation, string bodyPart) {
            Debug.Log(amt + " | " + bodyPart);
            ShowTakeDamage();
            switch (bodyPart) {
                case "Chest":
                    health -= (amt / 2f);
                    if (health > 0) {
                        bleeding.Add(StartCoroutine(TakeDamageContinuously(amt / 2.5f, 1.1f)));
                    }
                    break;
                case "Head":
                    health = 0;
                    break;
                case "Stomach":
                    health -= (amt / 3f);
                    if (health > 0) {
                        bleeding.Add(StartCoroutine(TakeDamageContinuously(amt / 3f, 1.25f)));
                    }
                    break;
                case "Leg":
                    playerMove.speedMultiplier = 2;
                    health -= (amt / 4f);
                    if (health > 0) {
                        bleeding.Add(StartCoroutine(TakeDamageContinuously(amt / 4f, 1.5f)));
                    }
                    break;
                case "Arm":
                    health -= (amt / 4f);
                    if (health > 0) {
                        bleeding.Add(StartCoroutine(TakeDamageContinuously(amt / 4f, 1.5f)));
                    }
                    break;
                case "Foot":
                    health -= (amt / 8f);
                    if (health > 0) {
                        bleeding.Add(StartCoroutine(TakeDamageContinuously(amt / 8f, 2f)));
                    }
                    break;
                case "All":
                    health -= amt;
                    break;
                default:
                    health -= amt;
                    break;
            }
            if (bleed) {
                GameObject blood = GameObject.Instantiate(bloodParticle, point, orientation, transform);
                bleedingObj.Add(blood);
            }
            if (photonView.isMine || singlePlayer) {
                healthBar.sizeDelta = new Vector2(health * barMultiplier, healthBar.sizeDelta.y);
            }
        }

        public void HealDamage(int woundsHealed) {
            if (Mathf.Max(bleedingObj.Count, bleeding.Count) < woundsHealed) {
                woundsHealed = Mathf.Max(bleedingObj.Count, bleeding.Count);
            }
            for (int i = 0; i < woundsHealed; i++) {
                StopCoroutine(bleeding[0]);
                bleeding.RemoveAt(0);
                Destroy(bleedingObj[0]);
                bleedingObj.RemoveAt(0);
            }
            playerMove.speedMultiplier = 1;
        }

        // TODO: improve needs system to avoid having explicit, similar methods for each need
        public void AddHealth(float amt) {
            health += amt;
            if (photonView.isMine || singlePlayer) {
                healthBar.sizeDelta = new Vector2(health * barMultiplier, healthBar.sizeDelta.y);
            }
        }

        public void AddHunger(float amt) {
            hunger += amt;
            if (photonView.isMine || singlePlayer) {
                hungerBar.sizeDelta = new Vector2(hunger * barMultiplier, hungerBar.sizeDelta.y);
            }
        }

        public void AddThirst(float amt) {
            thirst += amt;
            if (photonView.isMine || singlePlayer) {
                thirstBar.sizeDelta = new Vector2(thirst * barMultiplier, thirstBar.sizeDelta.y);
            }
        }

        public void AddWarmth(float amt) {
            warmth += amt;
            if (photonView.isMine || singlePlayer) {
                warmthBar.sizeDelta = new Vector2(warmth * barMultiplier, warmthBar.sizeDelta.y);
            }
        }

        public void AddStamina(float amt) {
            stamina += amt;
            if (photonView.isMine || singlePlayer) {
                staminaBar.sizeDelta = new Vector2(stamina * barMultiplier, staminaBar.sizeDelta.y);
            }
        }

        public float GetStamina() {
            return stamina;
        }

        System.Collections.IEnumerator TakeDamageContinuously(float amt, float time) {
            while (health > 0) {
                yield return new WaitForSeconds(time);
                //ShowTakeDamage();
                health -= amt;
                if (photonView.isMine || singlePlayer) {
                    healthBar.sizeDelta = new Vector2(health * barMultiplier, healthBar.sizeDelta.y);
                }
            }
        }

        void ShowTakeDamage() {
            System.Random random = new System.Random();
            int audio = random.Next(0, damageAudio.Length);
            src.clip = damageAudio[audio];
            src.PlayOneShot(src.clip);
            if (photonView.isMine || singlePlayer) {
                bleedPanel.SetActive(true);
                Invoke("StopBleedPanel", 1.5f);
            }
        }

        void StopBleedPanel() {
            bleedPanel.SetActive(false);
        }

        string OrdinalForm(int num) {
            int ones = num % 10;
            int tens = (num / 10) % 10;
            string suffix = "";
            if (tens == 1) {
                suffix = "th";
            } else {
                switch (ones) {
                    case 1:
                        suffix = "st";
                        break;
                    case 2:
                        suffix = "nd";
                        break;
                    case 3:
                        suffix = "rd";
                        break;
                    default:
                        suffix = "th";
                        break;
                }
            }
            return num.ToString() + suffix;
        }
    }
}