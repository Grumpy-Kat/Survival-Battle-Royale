using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CrimsonPlague.Items;

namespace CrimsonPlague.Managers {
    public class InventoryManager : Photon.MonoBehaviour {
        [SerializeField]
        bool spawnInventory = true;

        Dictionary<string, int> inventory = new Dictionary<string, int>();
        public Dictionary<string, List<Item>> inventoryItems = new Dictionary<string, List<Item>>();
        Dictionary<string, Text> inventoryObjs = new Dictionary<string, Text>();

        //[HideInInspector]
        public Player.Player player;

        public GameObject itemPrefab;

        public Transform objectInWorldParent { get; protected set; }

        [HideInInspector]
        public GameObject panel;
        public Button openInventoryBtn { get; protected set; }
        public Button closeInventoryBtn { get; protected set; }
        ScrollRect scroll;
        Text itemName;
        Image itemIcon;
        Text itemInfo;
        Text itemShortInfo;

        public Item currSelectedItem { get; protected set; }
        Button currSelectedBtn = null;

        int currSelectedIndex = -1;

        // MaxSpawn is always 1 less than the number written, due to exclusive random generation

        public int minSpawnWeapons = 1;
        public int maxSpawnWeapons = 4;

        public int minSpawnFood = 3;
        public int maxSpawnFood = 8;

        public int minSpawnMedicine = 1;
        public int maxSpawnMedicine = 4;

        public int minSpawnTools = 2;
        public int maxSpawnTools = 5;

        void Start() {
            if (player == null) {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject playerObj in players) {
                    CrimsonPlague.Player.Player playerComp = playerObj.GetComponentInChildren<CrimsonPlague.Player.Player>();
                    if (playerComp.photonView.owner.ID == photonView.owner.ID) {
                        player = playerComp;
                    }
                }
            }
            objectInWorldParent = GameObject.Find("ObjectsInWorld").transform;
            scroll = GameObject.Find("Canvas").transform.Find("Inventory").GetChild(0).GetComponent<ScrollRect>();
            openInventoryBtn = GameObject.Find("Canvas").transform.Find("OpenInventoryButton").GetComponent<Button>();
            closeInventoryBtn = scroll.transform.GetChild(2).GetComponent<Button>();
            closeInventoryBtn.onClick.Invoke();
            panel = scroll.transform.GetChild(0).GetChild(0).gameObject;
            itemName = GameObject.Find("Canvas").transform.Find("Inventory").GetChild(1).GetChild(0).Find("Name").GetComponent<Text>();
            itemIcon = itemName.transform.parent.Find("Icon").GetComponent<Image>();
            itemInfo = itemName.transform.parent.Find("Info").GetComponent<Text>();
            itemShortInfo = GameObject.Find("Canvas").transform.Find("ItemShortInfo").GetComponent<Text>();
            if (spawnInventory) {
                if (photonView.isMine || player.singlePlayer) {
                    ItemManager.Instance.SpawnInventory(this, photonView.owner.ID + ((int)PhotonNetwork.room.CustomProperties["s"] * photonView.owner.ID), true);
                } else {
                    ItemManager.Instance.SpawnInventory(this, photonView.owner.ID + ((int)PhotonNetwork.room.CustomProperties["s"] * photonView.owner.ID), false);
                }
            }
        }

        public void Init() {
            Start();
        }

        void Update() {
            if ((photonView.isMine || player.singlePlayer) && player.ready && (!player.useMap || !MapManager.Instance.map.activeInHierarchy)) {
                if (Input.GetButtonDown("Inventory")) {
                    if (openInventoryBtn != null) {
                        if (openInventoryBtn.IsActive()) {
                            openInventoryBtn.onClick.Invoke();
                            UpdateInfo(currSelectedItem);
                        } else {
                            if (closeInventoryBtn != null) {
                                closeInventoryBtn.onClick.Invoke();
                            }
                        }
                    }
                }
            }
            if (panel.activeInHierarchy && (photonView.isMine || player.singlePlayer)) {
                if (Input.GetButtonDown("NavigateInventory") && Input.GetAxis("NavigateInventory") > 0.1f) {
                    currSelectedIndex--;
                    if (currSelectedIndex < -1) {
                        currSelectedIndex = inventory.Count - 1;
                    } else if (currSelectedIndex > inventory.Count - 1) {
                        currSelectedIndex = -1;
                    }
                    UpdateInventory();
                } else if (Input.GetButtonDown("NavigateInventory") && Input.GetAxis("NavigateInventory") < -0.1f) {
                    currSelectedIndex++;
                    if (currSelectedIndex < -1) {
                        currSelectedIndex = inventory.Count - 1;
                    } else if (currSelectedIndex > inventory.Count - 1) {
                        currSelectedIndex = -1;
                    }
                    UpdateInventory();
                }
            }
            if (currSelectedItem != null) {
                itemShortInfo.text = currSelectedItem.DisplayShortInfo();
            } else {
                itemShortInfo.text = "";
            }
        }

        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

        public Item GetItem(string name) {
            if (inventoryItems.ContainsKey(name) && inventoryItems[name].Count > 0) {
                return inventoryItems[name][0];
            }
            return null;
        }

        void UpdateInventory() {
            if (currSelectedIndex < 0) {
                SelectItem(null, null);
            } else {
                string itemName = inventoryObjs.Keys.ToList()[currSelectedIndex];
                inventoryObjs[itemName].GetComponentInParent<Button>().onClick.Invoke();
            }
            scroll.verticalScrollbar.value = 1 - ((float)currSelectedIndex / (inventory.Count - 1));
        }

        public void AddItem(Item item) {
            photonView.RPC("AddItemRPC", PhotonTargets.AllViaServer, GetItemIdInWorld(item));
        }

        [PunRPC]
        void AddItemRPC(int itemId) {
            Item item = GetItemByIdInWorld(itemId);
            if (item != null) {
                if (player != null) {
                    item.gameObject.transform.SetParent(player.hand);
                }
                string name = item.itemName;
                item.inventory = this;
                item.IsHolding = false;
                item.IsOnGround = false;
                item.isMoving = false;
                item.gameObject.transform.localPosition = item.offset;
                item.gameObject.transform.localRotation = item.rot;
                if (inventory.ContainsKey(name)) {
                    Debug.Log("item exists");
                    if (inventory[name] == 0) {
                        inventoryObjs[name].transform.parent.gameObject.SetActive(true);
                    }
                    inventory[name]++;
                    inventoryItems[name].Add(item);
                    if (photonView.isMine || player.singlePlayer) {
                        inventoryObjs[name].text = inventory[name].ToString();
                    }
                } else {
                    inventory.Add(name, 1);
                    inventoryItems.Add(name, new List<Item>());
                    inventoryItems[name].Add(item);
                    if (photonView.isMine || player.singlePlayer) {
                        GameObject obj = GameObject.Instantiate(itemPrefab, new Vector3(250, (40 * inventoryObjs.Keys.Count) + 150, 0), itemPrefab.transform.rotation);
                        obj.transform.SetParent(panel.transform);
                        obj.GetComponent<Button>().onClick.AddListener(() => { SelectItem(item, obj.GetComponent<Button>()); });
                        obj.transform.GetChild(1).GetComponent<Text>().text = name;
                        inventoryObjs.Add(name, obj.transform.GetChild(0).GetComponent<Text>());
                    }
                }
            }
        }

        public void AddItemNonRPC(Item item) {
            if (player != null) {
                item.gameObject.transform.SetParent(player.hand);
            }
            item.gameObject.transform.localPosition = item.offset;
            item.gameObject.transform.localRotation = item.rot;
            string name = item.itemName;
            item.inventory = this;
            if (inventory.ContainsKey(name)) {
                if (inventory[name] == 0) {
                    inventoryObjs[name].transform.parent.gameObject.SetActive(true);
                }
                inventory[name]++;
                inventoryItems[name].Add(item);
                if (photonView.isMine || player.singlePlayer) {
                    inventoryObjs[name].text = inventory[name].ToString();
                }
            } else {
                inventory.Add(name, 1);
                inventoryItems.Add(name, new List<Item>());
                inventoryItems[name].Add(item);
                if (photonView.isMine || player.singlePlayer) {
                    GameObject obj = GameObject.Instantiate(itemPrefab, new Vector3(250, (40 * inventoryObjs.Keys.Count) + 150, 0), itemPrefab.transform.rotation);
                    obj.transform.SetParent(panel.transform);
                    obj.GetComponent<Button>().onClick.AddListener(() => { SelectItem(item, obj.GetComponent<Button>()); });
                    obj.transform.GetChild(1).GetComponent<Text>().text = name;
                    inventoryObjs.Add(name, obj.transform.GetChild(0).GetComponent<Text>());
                }
            }
        }

        public void RemoveItem(Item item) {
            int itemId = -1;
            int subId = -1;
            GetItemId(item, out itemId, out subId);
            photonView.RPC("RemoveItemActual", PhotonTargets.AllViaServer, itemId, subId);
        }

        [PunRPC]
        void RemoveItemActual(int itemId, int subId) {
            Item item = GetItemById(itemId, subId);
            if (item != null) {
                string name = item.itemName;
                if (inventory.ContainsKey(name)) {
                    inventory[name]--;
                    item.transform.SetParent(objectInWorldParent);
                    if (photonView.isMine || player.singlePlayer) {
                        inventoryObjs[name].text = inventory[name].ToString();
                        if (currSelectedItem == item && inventory[name] > 0) {
                            Button btn = inventoryObjs[name].GetComponentInParent<Button>();
                            btn.onClick.RemoveAllListeners();
                            btn.onClick.AddListener(() => { SelectItem(inventoryItems[name][0], btn); });
                            int itemSelectId = -1;
                            int subSelectId = -1;
                            GetItemId(inventoryItems[name][0], out itemId, out subId);
                            SelectItemActual(itemSelectId, subSelectId, GetButtonId(btn));
                        }
                    }
                    if (inventory[name] <= 0) {
                        if (photonView.isMine || player.singlePlayer) {
                            if (currSelectedItem == item) {
                                SelectItem(null, null);
                            }
                            if (currSelectedIndex >= inventoryItems.Values.ToList().IndexOf(new List<Item>() { item })) {
                                currSelectedIndex--;
                            }
                            Destroy(inventoryObjs[name].transform.parent.gameObject);
                        }
                        inventory.Remove(name);
                        inventoryItems.Remove(name);
                        if (photonView.isMine || player.singlePlayer) {
                            inventoryObjs.Remove(name);
                            List<Text> values = inventoryObjs.Values.ToList();
                            for (int i = 0; i < inventoryObjs.Values.Count; i++) {
                                values[i].transform.parent.position = new Vector3(250, (40 * i) + 150, 0);
                            }
                        }
                    } else {
                        inventoryItems[name].Remove(item);
                    }
                }
            }
        }

        public void SelectItem(Item item, Button button) {
            int itemId = -1;
            int subId = -1;
            GetItemId(item, out itemId, out subId);
            photonView.RPC("SelectItemActual", PhotonTargets.AllViaServer, itemId, subId, GetButtonId(button));
        }

        [PunRPC]
        void SelectItemActual(int itemId, int subId, int buttonId) {
            Item item = GetItemById(itemId, subId);
            Button button = GetButtonById(buttonId);
            if (currSelectedItem != null) {
                currSelectedItem.IsHolding = false;
            }
            if (currSelectedBtn != null) {
                currSelectedBtn.interactable = true;
            }
            if (item != null) {
                item.IsHolding = true;
                if (photonView.isMine || player.singlePlayer) {
                    UpdateInfo(item);
                }
            } else if (photonView.isMine || player.singlePlayer) {
                itemInfo.text = "";
                itemName.text = "";
            }
            if (button != null) {
                button.interactable = false;
            }
            currSelectedItem = item;
            currSelectedBtn = button;
        }

        void UpdateInfo(Item item) {
            if (item != null) {
                itemName.text = item.itemName;
                itemIcon.sprite = item.icon;
                itemInfo.text = item.DisplayInfo();
            }
        }

        public void DestroyItem(Item item) {
            int itemId = -1;
            int subId = -1;
            GetItemId(item, out itemId, out subId);
            photonView.RPC("DestroyItemActual", PhotonTargets.AllViaServer, itemId, subId);
        }

        [PunRPC]
        void DestroyItemActual(int itemId, int subId) {
            Item item = GetItemById(itemId, subId);
            if (item != null) {
                item.End();
                RemoveItemActual(itemId, subId);
                item.DestroyActual();
            }
        }

        public void DropItem(Item item) {
            int itemId = -1;
            int subId = -1;
            GetItemId(item, out itemId, out subId);
            photonView.RPC("DropItemActual", PhotonTargets.AllViaServer, itemId, subId);
        }

        [PunRPC]
        void DropItemActual(int itemId, int subId) {
            Item item = GetItemById(itemId, subId);
            if (item != null) {
                item.End();
                RemoveItemActual(itemId, subId);
                item.DropActual();
            }
        }

        void GetItemId(Item item, out int itemId, out int subId) {
            if (item == null) {
                itemId = -1;
                subId = -1;
                return;
            }
            List<List<Item>> values = inventoryItems.Values.ToList();
            for (int i = 0; i < values.Count; i++) {
                List<Item> items = values[i];
                if (items[0].itemName == item.itemName) {
                    itemId = i;
                    subId = items.IndexOf(item);
                    return;
                }
            }
            itemId = -1;
            subId = -1;
        }

        int GetItemIdInWorld(Item item) {
            if (item == null) {
                return -1;
            }
            Item[] worldItems = objectInWorldParent.GetComponentsInChildren<Item>();
            for (int i = 0; i < worldItems.Length; i++) {
                if (worldItems[i] == item) {
                    return i;
                }
            }
            return -1;
        }

        int GetButtonId(Button button) {
            if (!(photonView.isMine || player.singlePlayer) || button == null) {
                return -1;
            }
            List<Text> values = inventoryObjs.Values.ToList();
            for (int i = 0; i < values.Count; i++) {
                if (values[i].GetComponentInParent<Button>() == button) {
                    return i;
                }
            }
            return -1;
        }

        Item GetItemById(int itemId, int subId) {
            if (itemId == -1 || subId == -1) {
                return null;
            }
            List<List<Item>> values = inventoryItems.Values.ToList();
            if (values.Count > itemId && values[itemId] != null) {
                return values[itemId][subId];
            }
            return null;
        }

        Item GetItemByIdInWorld(int itemId) {
            if (itemId == -1) {
                return null;
            }
            Item[] worldItems = objectInWorldParent.GetComponentsInChildren<Item>();
            if (worldItems.Length > itemId) {
                return worldItems[itemId];
            }
            return null;
        }

        Button GetButtonById(int buttonId) {
            if (buttonId == -1 || !(photonView.isMine || player.singlePlayer)) {
                return null;
            }
            List<Text> values = inventoryObjs.Values.ToList();
            if (values.Count > buttonId && values[buttonId] != null) {
                return values[buttonId].GetComponentInParent<Button>();
            }
            return null;
        }
    }
}