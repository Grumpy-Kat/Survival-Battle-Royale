using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using CrimsonPlague.Managers;

namespace CrimsonPlague.Animals {
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    public class Animal : Photon.MonoBehaviour {
        public class DropItem {
            public string name;
            public string type;

            public DropItem(string name, string type) {
                this.name = name;
                this.type = type;
            }
        }

        [HideInInspector]
        public string animalName;
        float health;

        public Movement movement { get; protected set; }
        public Response response { get; protected set; }

        public Animator animator { get; protected set; }
        protected Transform objectInWorldParent;

        public float spawnChance;

        protected List<DropItem> dropItems;

        void Start() {
            animator = gameObject.GetComponent<Animator>();
            objectInWorldParent = GameObject.Find("ObjectsInWorld").transform;
            transform.tag = "Animal";
            if (movement != null) {
                switch (movement.subType) {
                    case MovementSubType.LandMovement:
                        ((LandMovement)movement).Start();
                        break;
                    case MovementSubType.AirMovement:
                        ((AirMovement)movement).Start();
                        break;
                    default:
                        movement.Start();
                        break;
                }
            }
            if (response != null) {
                switch (response.subType) {
                    case ResponseSubType.ExtraAggressiveResponse:
                        ((ExtraAggressiveResponse)response).Start();
                        break;
                    case ResponseSubType.AggressiveResponse:
                        ((AggressiveResponse)response).Start();
                        break;
                    case ResponseSubType.PassiveResponse:
                        ((PassiveResponse)response).Start();
                        break;
                    default:
                        response.Start();
                        break;
                }
            }
        }

        void Update() {
            if (movement != null) {
                switch (movement.subType) {
                    case MovementSubType.LandMovement:
                        ((LandMovement)movement).Update();
                        break;
                    case MovementSubType.AirMovement:
                        ((AirMovement)movement).Update();
                        break;
                    default:
                        movement.Update();
                        break;
                }
            }
            if (response != null) {
                switch (response.subType) {
                    case ResponseSubType.ExtraAggressiveResponse:
                        ((ExtraAggressiveResponse)response).Update();
                        break;
                    case ResponseSubType.AggressiveResponse:
                        ((AggressiveResponse)response).Update();
                        break;
                    case ResponseSubType.PassiveResponse:
                        ((PassiveResponse)response).Update();
                        break;
                    default:
                        response.Start();
                        break;
                }
            }
        }

        public void Load() {
            TextAsset animals = Resources.Load<TextAsset>("Data/Animals");
            XmlTextReader reader = new XmlTextReader(new StringReader(animals.text));
            if (reader.ReadToDescendant("Animals")) {
                if (reader.ReadToDescendant("Animal")) {
                    do {
                        if (reader.GetAttribute("animalName") == animalName) {
                            Load(reader);
                            break;
                        }
                    } while (reader.ReadToNextSibling("Animal"));
                }
            }
        }

        public void Load(XmlReader reader) {
            animalName = reader.GetAttribute("animalName");
            health = float.Parse(reader.GetAttribute("health"));
            dropItems = new List<DropItem>();
            reader.ReadToDescendant("AnimalProperty");
            do {
                //Debug.Log("During AnimalProperty: " + reader.ReadOuterXml());
                switch (reader.GetAttribute("type")) {
                    case "LandMovement":
                        movement = new LandMovement(this, reader);
                        break;
                    case "AirMovement":
                        movement = new AirMovement(this, reader);
                        break;
                    case "ExtraAggressiveResponse":
                        response = new ExtraAggressiveResponse(this, reader);
                        break;
                    case "AggressiveResponse":
                        response = new AggressiveResponse(this, reader);
                        break;
                    case "PassiveResponse":
                        response = new PassiveResponse(this, reader);
                        break;
                    case "AnimalDrop":
                        int count = Random.Range(int.Parse(reader.GetAttribute("minDrop")), int.Parse(reader.GetAttribute("maxDrop")));
                        for (int i = 0; i < count; i++) {
                            dropItems.Add(new DropItem(reader.GetAttribute("itemName"), reader.GetAttribute("dropType")));
                        }
                        break;
                    default:
                        Debug.Log(reader.ReadOuterXml());
                        break;
                }
            } while (reader.ReadToNextSibling("AnimalProperty"));
        }

        public void TakeDamage(float amt, CrimsonPlague.Player.Player attacker) {
            health -= amt;
            if (health <= 0) {
                DropItems();
                PhotonNetwork.Destroy(gameObject);
            }
            if (response != null) {
                switch (response.subType) {
                    case ResponseSubType.ExtraAggressiveResponse:
                        ((ExtraAggressiveResponse)response).TakeDamage(attacker);
                        break;
                    case ResponseSubType.AggressiveResponse:
                        ((AggressiveResponse)response).TakeDamage(attacker);
                        break;
                    case ResponseSubType.PassiveResponse:
                        ((PassiveResponse)response).TakeDamage(attacker);
                        break;
                    default:
                        response.TakeDamage(attacker);
                        break;
                }
            }
        }

        public void TakeDamage(float amt, Animal attacker) {
            if (attacker == this) {
                return;
            }
            health -= amt;
            if (health <= 0) {
                DropItems();
                PhotonNetwork.Destroy(gameObject);
            }
            if (response != null) {
                switch (response.subType) {
                    case ResponseSubType.ExtraAggressiveResponse:
                        ((ExtraAggressiveResponse)response).TakeDamage(attacker);
                        break;
                    case ResponseSubType.AggressiveResponse:
                        ((AggressiveResponse)response).TakeDamage(attacker);
                        break;
                    case ResponseSubType.PassiveResponse:
                        ((PassiveResponse)response).TakeDamage(attacker);
                        break;
                    default:
                        response.TakeDamage(attacker);
                        break;
                }
            }
        }

        void DropItems() {
            for (int i = 0; i < dropItems.Count; i++) {
                if (dropItems[i] != null) {
                    ItemManager.Instance.photonView.RPC("InstantiateObjectInWorld", PhotonTargets.AllViaServer, dropItems[i].type, dropItems[i].name, transform.position);
                    dropItems[i] = null;
                }
            }
        }

        public void Attack(CrimsonPlague.Player.Player prey) {
            if (response != null) {
                switch (response.subType) {
                    case ResponseSubType.ExtraAggressiveResponse:
                        ((ExtraAggressiveResponse)response).Attack(prey);
                        break;
                    case ResponseSubType.AggressiveResponse:
                        ((AggressiveResponse)response).Attack(prey);
                        break;
                    case ResponseSubType.PassiveResponse:
                        ((PassiveResponse)response).Attack(prey);
                        break;
                    default:
                        response.Attack(prey);
                        break;
                }
            }
        }

        public void Attack(Animal prey) {
            if (response != null) {
                switch (response.subType) {
                    case ResponseSubType.ExtraAggressiveResponse:
                        ((ExtraAggressiveResponse)response).Attack(prey);
                        break;
                    case ResponseSubType.AggressiveResponse:
                        ((AggressiveResponse)response).Attack(prey);
                        break;
                    case ResponseSubType.PassiveResponse:
                        ((PassiveResponse)response).Attack(prey);
                        break;
                    default:
                        response.Attack(prey);
                        break;
                }
            }
        }
    }
}