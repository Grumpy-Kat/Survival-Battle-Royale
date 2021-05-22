using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using CrimsonPlague.Player;
using CrimsonPlague.Animals;
using CrimsonPlague.Managers;

namespace CrimsonPlague.Items {
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour {
        [HideInInspector]
        public string itemName;
        [HideInInspector]
        public float speedMultiplier;
        public Sprite icon;

        [HideInInspector]
        public Vector3 offset;
        [HideInInspector]
        public Quaternion rot;

        [HideInInspector]
        public bool isMoving = false;
        bool isOnGround = false;
        public bool IsOnGround {
            get {
                return isOnGround;
            }
            set {
                isOnGround = value;
                if (isOnGround) {
                    gameObject.SetActive(true);
                } else if (!IsHolding) {
                    gameObject.SetActive(false);
                }
            }
        }

        bool isHolding = false;
        public bool IsHolding {
            get {
                return isHolding;
            }
            set {
                isHolding = value;
                if (isHolding) {
                    gameObject.SetActive(true);
                    if (playerMove != null) {
                        playerMove.itemSpeedMultiplier = speedMultiplier;
                    }
                } else if (!IsOnGround) {
                    if (playerMove != null) {
                        playerMove.itemSpeedMultiplier = 1;
                    }
                    gameObject.SetActive(false);
                }
            }
        }

        public string main;
        public float spawnChance;
        public float maxSpawn;

        [HideInInspector]
        public List<ItemType> types = new List<ItemType>();

        public PlayerMove playerMove {
            get {
                if (inventory != null) {
                    return inventory.player.playerMove;
                }
                return null;
            }
            set { }
        }
        [HideInInspector]
        public InventoryManager inventory;
        public AudioSource src { get; protected set; }

        void Start() {
            src = GetComponent<AudioSource>();
        }

        void OnEnable() {
            isMoving = false;
            foreach (ItemType type in types) {
                switch (type.subType) {
                    case ItemSubType.Weapon:
                        switch (((Weapon)type).weaponSubType) {
                            case WeaponSubType.GunWeapon:
                                ((GunWeapon)type).Start();
                                break;
                            case WeaponSubType.CloseRangeWeapon:
                                ((CloseRangeWeapon)type).Start();
                                break;
                            case WeaponSubType.ThrowableWeapon:
                                ((ThrowableWeapon)type).Start();
                                break;
                            default:
                                ((Weapon)type).Start();
                                break;
                        }
                        break;
                    case ItemSubType.Food:
                        ((Food)type).Start();
                        break;
                    case ItemSubType.Medicine:
                        ((Medicine)type).Start();
                        break;
                    case ItemSubType.Clothing:
                        ((Clothing)type).Start();
                        break;
                    case ItemSubType.Tool:
                        switch (((Tool)type).toolSubType) {
                            case ToolSubType.LightTool:
                                ((LightTool)type).Start();
                                break;
                            case ToolSubType.AmmoTool:
                                ((AmmoTool)type).Start();
                                break;
                            default:
                                ((Tool)type).Start();
                                break;
                        }
                        break;
                    case ItemSubType.Necessity:
                        ((Necessity)type).Start();
                        break;
                    default:
                        type.Start();
                        break;
                }
            }
        }

        void OnDisable() {
            End();
        }

        public void End() {
            foreach (ItemType type in types) {
                switch (type.subType) {
                    case ItemSubType.Weapon:
                        switch (((Weapon)type).weaponSubType) {
                            case WeaponSubType.GunWeapon:
                                ((GunWeapon)type).End();
                                break;
                            case WeaponSubType.CloseRangeWeapon:
                                ((CloseRangeWeapon)type).End();
                                break;
                            case WeaponSubType.ThrowableWeapon:
                                ((ThrowableWeapon)type).End();
                                break;
                            default:
                                ((Weapon)type).End();
                                break;
                        }
                        break;
                    case ItemSubType.Food:
                        ((Food)type).End();
                        break;
                    case ItemSubType.Medicine:
                        ((Medicine)type).End();
                        break;
                    case ItemSubType.Clothing:
                        ((Clothing)type).End();
                        break;
                    case ItemSubType.Tool:
                        switch (((Tool)type).toolSubType) {
                            case ToolSubType.LightTool:
                                ((LightTool)type).End();
                                break;
                            case ToolSubType.AmmoTool:
                                ((AmmoTool)type).End();
                                break;
                            default:
                                ((Tool)type).End();
                                break;
                        }
                        break;
                    case ItemSubType.Necessity:
                        ((Necessity)type).End();
                        break;
                    default:
                        type.End();
                        break;
                }
                type.animators = null;
            }
        }

        void Update() {
            if (!isMoving && !IsOnGround) {
                transform.localPosition = offset;
                transform.localRotation = rot;
            }
            if (!IsOnGround && (inventory != null && (!inventory.photonView.isMine || !inventory.panel.activeInHierarchy))) {
                foreach (ItemType type in types) {
                    switch (type.subType) {
                        case ItemSubType.Weapon:
                            switch (((Weapon)type).weaponSubType) {
                                case WeaponSubType.GunWeapon:
                                    ((GunWeapon)type).Update();
                                    break;
                                case WeaponSubType.CloseRangeWeapon:
                                    ((CloseRangeWeapon)type).Update();
                                    break;
                                case WeaponSubType.ThrowableWeapon:
                                    ((ThrowableWeapon)type).Update();
                                    break;
                                default:
                                    ((Weapon)type).Update();
                                    break;
                            }
                            break;
                        case ItemSubType.Food:
                            ((Food)type).Update();
                            break;
                        case ItemSubType.Medicine:
                            ((Medicine)type).Update();
                            break;
                        case ItemSubType.Clothing:
                            ((Clothing)type).Update();
                            break;
                        case ItemSubType.Tool:
                            switch (((Tool)type).toolSubType) {
                                case ToolSubType.LightTool:
                                    ((LightTool)type).Update();
                                    break;
                                case ToolSubType.AmmoTool:
                                    ((AmmoTool)type).Update();
                                    break;
                                default:
                                    ((Tool)type).Update();
                                    break;
                            }
                            break;
                        case ItemSubType.Necessity:
                            ((Necessity)type).Update();
                            break;
                        default:
                            type.Update();
                            break;
                    }
                }
                if (Input.GetButtonDown("Fire1")) {
                    foreach (ItemType type in types) {
                        switch (type.subType) {
                            case ItemSubType.Weapon:
                                switch (((Weapon)type).weaponSubType) {
                                    case WeaponSubType.GunWeapon:
                                        ((GunWeapon)type).DoAction();
                                        break;
                                    case WeaponSubType.CloseRangeWeapon:
                                        ((CloseRangeWeapon)type).DoAction();
                                        break;
                                    case WeaponSubType.ThrowableWeapon:
                                        ((ThrowableWeapon)type).DoAction();
                                        break;
                                    default:
                                        ((Weapon)type).DoAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Food:
                                ((Food)type).DoAction();
                                break;
                            case ItemSubType.Medicine:
                                ((Medicine)type).DoAction();
                                break;
                            case ItemSubType.Clothing:
                                ((Clothing)type).DoAction();
                                break;
                            case ItemSubType.Tool:
                                switch (((Tool)type).toolSubType) {
                                    case ToolSubType.LightTool:
                                        ((LightTool)type).DoAction();
                                        break;
                                    case ToolSubType.AmmoTool:
                                        ((AmmoTool)type).DoAction();
                                        break;
                                    default:
                                        ((Tool)type).DoAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Necessity:
                                ((Necessity)type).DoAction();
                                break;
                            default:
                                type.DoAction();
                                break;
                        }
                    }
                } else if (Input.GetButton("Fire1")) {
                    foreach (ItemType type in types) {
                        switch (type.subType) {
                            case ItemSubType.Weapon:
                                switch (((Weapon)type).weaponSubType) {
                                    case WeaponSubType.GunWeapon:
                                        ((GunWeapon)type).HoldAction();
                                        break;
                                    case WeaponSubType.CloseRangeWeapon:
                                        ((CloseRangeWeapon)type).HoldAction();
                                        break;
                                    case WeaponSubType.ThrowableWeapon:
                                        ((ThrowableWeapon)type).HoldAction();
                                        break;
                                    default:
                                        ((Weapon)type).HoldAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Food:
                                ((Food)type).HoldAction();
                                break;
                            case ItemSubType.Medicine:
                                ((Medicine)type).HoldAction();
                                break;
                            case ItemSubType.Clothing:
                                ((Clothing)type).HoldAction();
                                break;
                            case ItemSubType.Tool:
                                switch (((Tool)type).toolSubType) {
                                    case ToolSubType.LightTool:
                                        ((LightTool)type).HoldAction();
                                        break;
                                    case ToolSubType.AmmoTool:
                                        ((AmmoTool)type).HoldAction();
                                        break;
                                    default:
                                        ((Tool)type).HoldAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Necessity:
                                ((Necessity)type).HoldAction();
                                break;
                            default:
                                type.HoldAction();
                                break;
                        }
                    }
                } else if (Input.GetButtonUp("Fire1")) {
                    foreach (ItemType type in types) {
                        switch (type.subType) {
                            case ItemSubType.Weapon:
                                switch (((Weapon)type).weaponSubType) {
                                    case WeaponSubType.GunWeapon:
                                        ((GunWeapon)type).FinishAction();
                                        break;
                                    case WeaponSubType.CloseRangeWeapon:
                                        ((CloseRangeWeapon)type).FinishAction();
                                        break;
                                    case WeaponSubType.ThrowableWeapon:
                                        ((ThrowableWeapon)type).FinishAction();
                                        break;
                                    default:
                                        ((Weapon)type).FinishAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Food:
                                ((Food)type).FinishAction();
                                break;
                            case ItemSubType.Medicine:
                                ((Medicine)type).FinishAction();
                                break;
                            case ItemSubType.Clothing:
                                ((Clothing)type).FinishAction();
                                break;
                            case ItemSubType.Tool:
                                switch (((Tool)type).toolSubType) {
                                    case ToolSubType.LightTool:
                                        ((LightTool)type).FinishAction();
                                        break;
                                    case ToolSubType.AmmoTool:
                                        ((AmmoTool)type).FinishAction();
                                        break;
                                    default:
                                        ((Tool)type).FinishAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Necessity:
                                ((Necessity)type).FinishAction();
                                break;
                            default:
                                type.FinishAction();
                                break;
                        }
                    }
                }
                if (Input.GetButtonDown("Fire2")) {
                    foreach (ItemType type in types) {
                        switch (type.subType) {
                            case ItemSubType.Weapon:
                                switch (((Weapon)type).weaponSubType) {
                                    case WeaponSubType.GunWeapon:
                                        ((GunWeapon)type).DoOtherAction();
                                        break;
                                    case WeaponSubType.CloseRangeWeapon:
                                        ((CloseRangeWeapon)type).DoOtherAction();
                                        break;
                                    case WeaponSubType.ThrowableWeapon:
                                        ((ThrowableWeapon)type).DoOtherAction();
                                        break;
                                    default:
                                        ((Weapon)type).DoOtherAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Food:
                                ((Food)type).DoOtherAction();
                                break;
                            case ItemSubType.Medicine:
                                ((Medicine)type).DoOtherAction();
                                break;
                            case ItemSubType.Clothing:
                                ((Clothing)type).DoOtherAction();
                                break;
                            case ItemSubType.Tool:
                                switch (((Tool)type).toolSubType) {
                                    case ToolSubType.LightTool:
                                        ((LightTool)type).DoOtherAction();
                                        break;
                                    case ToolSubType.AmmoTool:
                                        ((AmmoTool)type).DoOtherAction();
                                        break;
                                    default:
                                        ((Tool)type).DoOtherAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Necessity:
                                ((Necessity)type).DoOtherAction();
                                break;
                            default:
                                type.DoOtherAction();
                                break;
                        }
                    }
                } else if (Input.GetButton("Fire2")) {
                    foreach (ItemType type in types) {
                        switch (type.subType) {
                            case ItemSubType.Weapon:
                                switch (((Weapon)type).weaponSubType) {
                                    case WeaponSubType.GunWeapon:
                                        ((GunWeapon)type).HoldOtherAction();
                                        break;
                                    case WeaponSubType.CloseRangeWeapon:
                                        ((CloseRangeWeapon)type).HoldOtherAction();
                                        break;
                                    case WeaponSubType.ThrowableWeapon:
                                        ((ThrowableWeapon)type).HoldOtherAction();
                                        break;
                                    default:
                                        ((Weapon)type).HoldOtherAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Food:
                                ((Food)type).HoldOtherAction();
                                break;
                            case ItemSubType.Medicine:
                                ((Medicine)type).HoldOtherAction();
                                break;
                            case ItemSubType.Clothing:
                                ((Clothing)type).HoldOtherAction();
                                break;
                            case ItemSubType.Tool:
                                switch (((Tool)type).toolSubType) {
                                    case ToolSubType.LightTool:
                                        ((LightTool)type).HoldOtherAction();
                                        break;
                                    case ToolSubType.AmmoTool:
                                        ((AmmoTool)type).HoldOtherAction();
                                        break;
                                    default:
                                        ((Tool)type).HoldOtherAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Necessity:
                                ((Necessity)type).HoldOtherAction();
                                break;
                            default:
                                type.HoldOtherAction();
                                break;
                        }
                    }
                } else if (Input.GetButtonUp("Fire2")) {
                    foreach (ItemType type in types) {
                        switch (type.subType) {
                            case ItemSubType.Weapon:
                                switch (((Weapon)type).weaponSubType) {
                                    case WeaponSubType.GunWeapon:
                                        ((GunWeapon)type).FinishOtherAction();
                                        break;
                                    case WeaponSubType.CloseRangeWeapon:
                                        ((CloseRangeWeapon)type).FinishOtherAction();
                                        break;
                                    case WeaponSubType.ThrowableWeapon:
                                        ((ThrowableWeapon)type).FinishOtherAction();
                                        break;
                                    default:
                                        ((Weapon)type).FinishOtherAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Food:
                                ((Food)type).FinishOtherAction();
                                break;
                            case ItemSubType.Medicine:
                                ((Medicine)type).FinishOtherAction();
                                break;
                            case ItemSubType.Clothing:
                                ((Clothing)type).FinishOtherAction();
                                break;
                            case ItemSubType.Tool:
                                switch (((Tool)type).toolSubType) {
                                    case ToolSubType.LightTool:
                                        ((LightTool)type).FinishOtherAction();
                                        break;
                                    case ToolSubType.AmmoTool:
                                        ((AmmoTool)type).FinishOtherAction();
                                        break;
                                    default:
                                        ((Tool)type).FinishOtherAction();
                                        break;
                                }
                                break;
                            case ItemSubType.Necessity:
                                ((Necessity)type).FinishOtherAction();
                                break;
                            default:
                                type.FinishOtherAction();
                                break;
                        }
                    }
                }
            }
        }

        public void AddToInventory(InventoryManager inventory) {
            this.inventory = inventory;
            IsOnGround = false;
            IsHolding = false;
        }


        public void RemoveFromInventory() {
            if (inventory != null) {
                inventory.RemoveItem(this);
                inventory = null;
            }
            IsHolding = false;
        }

        public void DestroyActual() {
            Destroy(gameObject);
        }

        public void DropActual() {
            IsOnGround = true;
        }

        public void Load() {
            TextAsset items = Resources.Load<TextAsset>("Data/Items");
            XmlTextReader reader = new XmlTextReader(new StringReader(items.text));
            if (reader.ReadToDescendant("Items")) {
                if (reader.ReadToDescendant("Item")) {
                    do {
                        if (reader.GetAttribute("itemName") == itemName) {
                            Load(reader);
                            break;
                        }
                    } while (reader.ReadToNextSibling("Item"));
                }
            }
        }

        public void Load(XmlReader reader) {
            itemName = reader.GetAttribute("itemName");
            speedMultiplier = float.Parse(reader.GetAttribute("speedMultiplier"));
            offset = new Vector3(float.Parse(reader.GetAttribute("offsetX")), float.Parse(reader.GetAttribute("offsetY")), float.Parse(reader.GetAttribute("offsetZ")));
            rot = Quaternion.Euler(new Vector3(float.Parse(reader.GetAttribute("rotX")), float.Parse(reader.GetAttribute("rotY")), float.Parse(reader.GetAttribute("rotZ"))));
            types = new List<ItemType>();
            reader.ReadToDescendant("ItemType");
            do {
                switch (reader.GetAttribute("type")) {
                    case "Weapon":
                        switch (reader.GetAttribute("subType")) {
                            case "GunWeapon":
                                types.Add(new GunWeapon(this, reader));
                                break;
                            case "CloseRangeWeapon":
                                types.Add(new CloseRangeWeapon(this, reader));
                                break;
                            case "ThrowableWeapon":
                                types.Add(new ThrowableWeapon(this, reader));
                                break;
                            default:
                                types.Add(new Weapon(this, WeaponSubType.Default, reader));
                                break;
                        }
                        break;
                    case "Food":
                        types.Add(new Food(this, reader));
                        break;
                    case "Medicine":
                        types.Add(new Medicine(this, reader));
                        break;
                    case "Clothing":
                        types.Add(new Clothing(this, reader));
                        break;
                    case "Tool":
                        switch (reader.GetAttribute("subType")) {
                            case "LightTool":
                                types.Add(new LightTool(this, reader));
                                break;
                            case "AmmoTool":
                                types.Add(new AmmoTool(this, reader));
                                break;
                            default:
                                types.Add(new Tool(this, ToolSubType.Default, reader));
                                break;
                        }
                        break;
                    case "Necessity":
                        types.Add(new Necessity(this, reader));
                        break;
                    default:
                        Debug.Log(reader.ReadOuterXml());
                        types.Add(new ItemType(this, ItemSubType.Default, reader));
                        break;
                }
            } while (reader.ReadToNextSibling("ItemType"));
        }

        void OnCollisionEnter(Collision col) {
            if (!IsOnGround) {
                foreach (ItemType type in types) {
                    switch (type.subType) {
                        case ItemSubType.Weapon:
                            switch (((Weapon)type).weaponSubType) {
                                case WeaponSubType.GunWeapon:
                                    ((GunWeapon)type).OnCollisionEnter(col);
                                    break;
                                case WeaponSubType.CloseRangeWeapon:
                                    ((CloseRangeWeapon)type).OnCollisionEnter(col);
                                    break;
                                case WeaponSubType.ThrowableWeapon:
                                    ((ThrowableWeapon)type).OnCollisionEnter(col);
                                    break;
                                default:
                                    ((Weapon)type).OnCollisionEnter(col);
                                    break;
                            }
                            break;
                        case ItemSubType.Food:
                            ((Food)type).OnCollisionEnter(col);
                            break;
                        case ItemSubType.Medicine:
                            ((Medicine)type).OnCollisionEnter(col);
                            break;
                        case ItemSubType.Clothing:
                            ((Clothing)type).OnCollisionEnter(col);
                            break;
                        case ItemSubType.Tool:
                            switch (((Tool)type).toolSubType) {
                                case ToolSubType.LightTool:
                                    ((LightTool)type).OnCollisionEnter(col);
                                    break;
                                case ToolSubType.AmmoTool:
                                    ((AmmoTool)type).OnCollisionEnter(col);
                                    break;
                                default:
                                    ((Tool)type).OnCollisionEnter(col);
                                    break;
                            }
                            break;
                        case ItemSubType.Necessity:
                            ((Necessity)type).OnCollisionEnter(col);
                            break;
                        default:
                            type.OnCollisionEnter(col);
                            break;
                    }
                }
            }
        }

        public string DisplayInfo() {
            string display = "Speed Multiplier: " + speedMultiplier + " \n Spawn Chance: " + spawnChance + " \n\n";
            foreach (ItemType type in types) {
                switch (type.subType) {
                    case ItemSubType.Weapon:
                        switch (((Weapon)type).weaponSubType) {
                            case WeaponSubType.GunWeapon:
                                display += ((GunWeapon)type).DisplayInfo();
                                break;
                            case WeaponSubType.CloseRangeWeapon:
                                display += ((CloseRangeWeapon)type).DisplayInfo();
                                break;
                            case WeaponSubType.ThrowableWeapon:
                                display += ((ThrowableWeapon)type).DisplayInfo();
                                break;
                            default:
                                display += ((Weapon)type).DisplayInfo();
                                break;
                        }
                        break;
                    case ItemSubType.Food:
                        display += ((Food)type).DisplayInfo();
                        break;
                    case ItemSubType.Medicine:
                        display += ((Medicine)type).DisplayInfo();
                        break;
                    case ItemSubType.Clothing:
                        display += ((Clothing)type).DisplayInfo();
                        break;
                    case ItemSubType.Tool:
                        switch (((Tool)type).toolSubType) {
                            case ToolSubType.LightTool:
                                display += ((LightTool)type).DisplayInfo();
                                break;
                            case ToolSubType.AmmoTool:
                                display += ((AmmoTool)type).DisplayInfo();
                                break;
                            default:
                                display += ((Tool)type).DisplayInfo();
                                break;
                        }
                        break;
                    case ItemSubType.Necessity:
                        display += ((Necessity)type).DisplayInfo();
                        break;
                    default:
                        display += type.DisplayInfo();
                        break;
                }
                display += " \n\n";
            }
            return display;
        }

        public string DisplayShortInfo() {
            string display = itemName;
            foreach (ItemType type in types) {
                display += " \n- ";
                switch (type.subType) {
                    case ItemSubType.Weapon:
                        switch (((Weapon)type).weaponSubType) {
                            case WeaponSubType.GunWeapon:
                                display += ((GunWeapon)type).DisplayShortInfo();
                                break;
                            case WeaponSubType.CloseRangeWeapon:
                                display += ((CloseRangeWeapon)type).DisplayShortInfo();
                                break;
                            case WeaponSubType.ThrowableWeapon:
                                display += ((ThrowableWeapon)type).DisplayShortInfo();
                                break;
                            default:
                                display += ((Weapon)type).DisplayShortInfo();
                                break;
                        }
                        break;
                    case ItemSubType.Food:
                        display += ((Food)type).DisplayShortInfo();
                        break;
                    case ItemSubType.Medicine:
                        display += ((Medicine)type).DisplayShortInfo();
                        break;
                    case ItemSubType.Clothing:
                        display += ((Clothing)type).DisplayShortInfo();
                        break;
                    case ItemSubType.Tool:
                        switch (((Tool)type).toolSubType) {
                            case ToolSubType.LightTool:
                                display += ((LightTool)type).DisplayShortInfo();
                                break;
                            case ToolSubType.AmmoTool:
                                display += ((AmmoTool)type).DisplayShortInfo();
                                break;
                            default:
                                display += ((Tool)type).DisplayShortInfo();
                                break;
                        }
                        break;
                    case ItemSubType.Necessity:
                        display += ((Necessity)type).DisplayShortInfo();
                        break;
                    default:
                        display += type.DisplayInfo();
                        break;
                }
            }
            return display;
        }

        public void TakeDamage(float amt, Transform obj, Vector3 point, Quaternion orientaton) {
            Debug.Log(amt + " | " + obj.name + " | " + obj.tag);
            if (obj.tag == "Animal") {
                Animal animal = obj.GetComponent<Animal>();
                animal.TakeDamage(amt, inventory.player);
                return;
            }
            if (obj.tag == "AnimalParent") {
                Animal animal = obj.parent.GetComponentInParent<Animal>();
                animal.TakeDamage(amt, inventory.player);
                return;
            }
            if (obj.tag == "PlayerBody" || obj.tag == "Clothes") {
                obj.tag = "Stomach";
            }
            if (obj.tag == "Stomach" || obj.tag == "Chest" || obj.tag == "Head" || obj.tag == "Arm" || obj.tag == "Leg" || obj.tag == "Foot") {
                Transform playerObj = GetParent(obj, "FG3D_Char_DeuHumans");
                if (playerObj != null) {
                    playerObj.GetComponent<CrimsonPlague.Player.Player>().TakeDamage(amt, true, point, orientaton, obj.tag);
                }
            }
        }

        public void SpawnNewObject(Vector3 pos, GameObject parent) {
            GameObject obj = GameObject.Instantiate(gameObject, pos, transform.rotation);
            obj.transform.SetParent(parent.transform);
        }

        public Transform GetParent(Transform child, string name) {
            Transform result = child.parent;
            if (result == null || (name != "Player" && result.name == "Player")) {
                return null;
            }
            if (result.name == name) {
                return result;
            }
            return GetParent(result, name);
        }
    }
}