using System.Xml;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using CrimsonPlague.Items;

namespace CrimsonPlague.Managers {
    public class ItemManager : Photon.MonoBehaviour {
        public class DropableItem {
            public GameObject obj;
            public Vector3 offset = Vector3.zero;
            public Quaternion rot = Quaternion.identity;
            public string itemName;
            public Item itemComp;

            public float spawnChance;
            public float maxSpawn;

            public float minProbability;
            public float maxProbability;

            public DropableItem(GameObject obj, Vector3 offset, Quaternion rot, string itemName, float minProbability, XmlReader reader) {
                this.obj = obj;
                this.offset = offset;
                this.rot = rot;
                this.itemName = itemName;
                itemComp = obj.GetComponent<Item>();
                itemComp.Load(reader);
                itemComp.IsHolding = false;
                spawnChance = itemComp.spawnChance;
                maxSpawn = itemComp.maxSpawn;
                this.minProbability = minProbability;
                maxProbability = minProbability + spawnChance;
            }
        }

        public static ItemManager Instance { get; protected set; }

        Dictionary<string, DropableItem> weaponObjs = new Dictionary<string, DropableItem>();
        Dictionary<string, DropableItem> foodObjs = new Dictionary<string, DropableItem>();
        Dictionary<string, DropableItem> medicineObjs = new Dictionary<string, DropableItem>();
        Dictionary<string, DropableItem> toolObjs = new Dictionary<string, DropableItem>();
        Dictionary<string, DropableItem> necessityObjs = new Dictionary<string, DropableItem>();

        [SerializeField]
        Transform itemParent;

        [SerializeField]
        float spawnRate = 20;
        [SerializeField]
        float spawnAmt = 9;

        float totalWeaponProbability = 0;
        float totalFoodProbability = 0;
        float totalMedicineProbability = 0;
        float totalToolProbability = 0;

        InventoryManager inventory;

        float timePassed = 0;

        System.Random updateRandom;

        void Awake() {
            Instance = this;
            updateRandom = new System.Random();
            TextAsset items = Resources.Load<TextAsset>("Data/Items");
            XmlTextReader reader = new XmlTextReader(new StringReader(items.text));
            totalWeaponProbability = 0;
            totalFoodProbability = 0;
            totalMedicineProbability = 0;
            totalToolProbability = 0;
            if (reader.ReadToDescendant("Items")) {
                if (reader.ReadToDescendant("Item")) {
                    do {
                        GameObject prefab = Resources.Load<GameObject>(reader.GetAttribute("prefab"));
                        Vector3 offset = new Vector3(float.Parse(reader.GetAttribute("offsetX")), float.Parse(reader.GetAttribute("offsetY")), float.Parse(reader.GetAttribute("offsetZ")));
                        Quaternion rot = Quaternion.Euler(new Vector3(float.Parse(reader.GetAttribute("rotX")), float.Parse(reader.GetAttribute("rotY")), float.Parse(reader.GetAttribute("rotZ"))));
                        GameObject obj = GameObject.Instantiate(prefab);
                        string name = reader.GetAttribute("itemName");
                        obj.name = name;
                        obj.transform.parent = itemParent;
                        switch (obj.GetComponent<Item>().main) {
                            case "Weapon":
                                weaponObjs.Add(name, new DropableItem(obj, offset, rot, name, totalWeaponProbability, reader));
                                totalWeaponProbability += weaponObjs[name].spawnChance;
                                break;
                            case "Food":
                                foodObjs.Add(name, new DropableItem(obj, offset, rot, name, totalFoodProbability, reader));
                                totalFoodProbability += foodObjs[name].spawnChance;
                                break;
                            case "Medicine":
                                medicineObjs.Add(name, new DropableItem(obj, offset, rot, name, totalMedicineProbability, reader));
                                totalMedicineProbability += medicineObjs[name].spawnChance;
                                break;
                            case "Tool":
                                toolObjs.Add(name, new DropableItem(obj, offset, rot, name, totalToolProbability, reader));
                                totalToolProbability += toolObjs[name].spawnChance;
                                break;
                            case "Necessity":
                                necessityObjs.Add(name, new DropableItem(obj, offset, rot, name, 0, reader));
                                break;
                            default:
                                Debug.Log(obj.GetComponent<Item>().main);
                                break;
                        }
                    } while (reader.ReadToNextSibling("Item"));
                }
            }
        }

        void Update() {
            if (inventory != null && inventory.photonView.isMine && PhotonNetwork.isMasterClient && inventory.player.ready) {
                timePassed += Time.deltaTime;
                if (timePassed >= spawnRate) {
                    float totalProbability = 0;
                    List<DropableItem> list;
                    string type = "";
                    int objSpawnType = updateRandom.Next(0, 4);
                    switch (objSpawnType) {
                        case 0:
                            totalProbability = totalWeaponProbability;
                            list = weaponObjs.Values.ToList();
                            type = "Weapon";
                            break;
                        case 1:
                            totalProbability = totalFoodProbability;
                            list = foodObjs.Values.ToList();
                            type = "Food";
                            break;
                        case 2:
                            totalProbability = totalMedicineProbability;
                            list = medicineObjs.Values.ToList();
                            type = "Medicine";
                            break;
                        case 3:
                            totalProbability = totalToolProbability;
                            list = toolObjs.Values.ToList();
                            type = "Tool";
                            break;
                        default:
                            totalProbability = totalWeaponProbability;
                            list = weaponObjs.Values.ToList();
                            type = "Weapon";
                            break;
                    }
                    SpawnObjectsInWorld(updateRandom, Mathf.CeilToInt(PhotonNetwork.playerList.Length * spawnAmt), totalProbability, list, type);
                    timePassed = 0;
                }
            }
        }

        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

        public int SpawnInventory(InventoryManager inventory, int seed = 0, bool useUI = true) {
            //Player starts with random inventory, decide what and how many objects to give them
            if (seed == 0) {
                seed = (int)(Time.time * 1000);
            }

            if (inventory.photonView.isMine && PhotonNetwork.isMasterClient) {
                this.inventory = inventory;
            } else if (this.inventory == null) {
                this.inventory = inventory;
            }

            System.Random random = new System.Random(seed);

            // Necessities, must give one of each so does not require random generation
            Debug.Log("Necessities: " + necessityObjs);
            foreach (DropableItem item in necessityObjs.Values.ToList()) {
                InstantiateObject(inventory, item, useUI);
            }

            // Weapons
            int spawnObjs = random.Next(inventory.minSpawnWeapons, inventory.maxSpawnWeapons);
            Debug.Log("Weapons: " + spawnObjs);
            PickObjects(inventory, random, spawnObjs, totalWeaponProbability, weaponObjs.Values.ToList(), useUI);

            // Food
            spawnObjs = random.Next(inventory.minSpawnFood, inventory.maxSpawnFood);
            Debug.Log("Food:" + spawnObjs);
            PickObjects(inventory, random, spawnObjs, totalFoodProbability, foodObjs.Values.ToList(), useUI);

            // Medicine
            spawnObjs = random.Next(inventory.minSpawnMedicine, inventory.maxSpawnMedicine);
            Debug.Log("Medicine: " + spawnObjs);
            PickObjects(inventory, random, spawnObjs, totalMedicineProbability, medicineObjs.Values.ToList(), useUI);

            // Tools
            spawnObjs = random.Next(inventory.minSpawnTools, inventory.maxSpawnTools);
            Debug.Log("Tools: " + spawnObjs);
            PickObjects(inventory, random, spawnObjs, totalToolProbability, toolObjs.Values.ToList(), useUI);

            return seed;
        }

        void PickObjects(InventoryManager inventory, System.Random random, int spawnObjs, float totalProbability, List<DropableItem> list, bool useUI) {
            // Uses weighted probability based on random spawn chance
            if (list != null) {
                Dictionary<DropableItem, int> spawnAmt = new Dictionary<DropableItem, int>();
                for (int i = 0; i < spawnObjs; i++) {
                    bool isValid = false;
                    int tries = 0;
                    DropableItem currItem = null;

                    do {
                        isValid = false;

                        float probability = (float)random.NextDouble() * totalProbability;

                        foreach (DropableItem item in list) {
                            if (item.minProbability <= probability && item.maxProbability >= probability) {
                                if (item.maxSpawn <= 0) {
                                    isValid = false;
                                    continue;
                                }

                                if (!spawnAmt.ContainsKey(item)) {
                                    spawnAmt.Add(item, 1);
                                    isValid = true;
                                    currItem = item;
                                    continue;
                                }

                                if (spawnAmt[item] > item.maxSpawn) {
                                    isValid = false;
                                    continue;
                                }

                                spawnAmt[item]++;
                                isValid = true;
                                currItem = item;
                            }
                        }

                        tries++;
                        if (tries > 5 && !isValid) {
                            isValid = true;
                            currItem = list[0];
                        }
                    } while (!isValid);

                    InstantiateObject(inventory, currItem, useUI);
                }
            }
        }

        void InstantiateObject(InventoryManager inventory, DropableItem item, bool useUI) {
            if (item != null) {
                GameObject obj = Instantiate(item.obj);
                if (useUI) {
                    obj.transform.SetParent(inventory.player.hand);
                }
                obj.name = item.itemName;
                obj.transform.localPosition = item.offset;
                obj.transform.localRotation = item.rot;
                Item itemComp = obj.GetComponent<Item>();
                itemComp.Load();
                itemComp.IsHolding = false;
                inventory.AddItemNonRPC(itemComp);
            }
        }

        void SpawnObjectsInWorld(System.Random random, int spawnObjs, float totalProbability, List<DropableItem> list, string type) {
            if (inventory != null && list != null) {
                for (int i = 0; i < spawnObjs; i++) {
                    int currItemId = 0;
                    float probability = (float)random.NextDouble() * totalProbability;
                    for (int j = 0; j < list.Count; j++) {
                        if (list[j].minProbability <= probability && list[j].maxProbability >= probability) {
                            currItemId = j;
                            break;
                        }
                    }
                    float x = random.Next(-485, 485);
                    float z = random.Next(-485, 485);
                    float y = 500;
                    RaycastHit hit;
                    Ray ray = new Ray(new Vector3(x, y, z), Vector3.down);
                    if (Physics.Raycast(ray, out hit)) {
                        y -= hit.distance;
                    }
                    photonView.RPC("InstantiateObjectInWorld", PhotonTargets.AllViaServer, type, currItemId, new Vector3(x, y, z));
                }
            }
        }

        [PunRPC]
        void InstantiateObjectInWorld(string type, int itemId, Vector3 pos) {
            DropableItem item = null;
            switch (type) {
                case "Weapon":
                    item = weaponObjs.Values.ToList()[itemId];
                    break;
                case "Food":
                    item = foodObjs.Values.ToList()[itemId];
                    break;
                case "Medicine":
                    item = medicineObjs.Values.ToList()[itemId];
                    break;
                case "Tool":
                    item = toolObjs.Values.ToList()[itemId];
                    break;
                default:
                    Debug.Log(type);
                    break;
            }
            if (item != null) {
                GameObject obj = Instantiate(item.obj);
                obj.transform.position = pos;
                obj.name = item.itemName;
                //Debug.Log(item.itemName);
                obj.transform.parent = inventory.objectInWorldParent;
                Item itemComp = obj.GetComponent<Item>();
                itemComp.Load();
                itemComp.IsHolding = false;
                itemComp.IsOnGround = true;
            }
        }

        [PunRPC]
        void InstantiateObjectInWorld(string type, string name, Vector3 pos) {
            DropableItem item = null;
            List<DropableItem> list;
            switch (type) {
                case "Weapon":
                    list = weaponObjs.Values.ToList();
                    break;
                case "Food":
                    list = foodObjs.Values.ToList();
                    break;
                case "Medicine":
                    list = medicineObjs.Values.ToList();
                    break;
                case "Tool":
                    list = toolObjs.Values.ToList();
                    break;
                default:
                    list = new List<DropableItem>();
                    Debug.Log(type);
                    break;
            }
            foreach (DropableItem listItem in list) {
                if (listItem.itemName == name) {
                    item = listItem;
                }
            }
            if (item != null) {
                GameObject obj = Instantiate(item.obj);
                obj.transform.position = pos;
                obj.name = item.itemName;
                //Debug.Log(item.itemName);
                obj.transform.parent = inventory.objectInWorldParent;
                Item itemComp = obj.GetComponent<Item>();
                itemComp.Load();
                itemComp.IsHolding = false;
                itemComp.IsOnGround = true;
            }
        }
    }
}