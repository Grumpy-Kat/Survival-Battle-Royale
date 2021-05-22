using System.Xml;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using CrimsonPlague.Animals;

namespace CrimsonPlague.Managers {
    public class AnimalManager : Photon.MonoBehaviour {
        public class SpawnableAnimal {
            public GameObject obj;
            public string animalName;
            public Animal animalComp;

            public float spawnChance;

            public float minProbability;
            public float maxProbability;

            public SpawnableAnimal(GameObject obj, string animalName, float minProbability, XmlReader reader) {
                this.obj = obj;
                this.animalName = animalName;
                animalComp = obj.GetComponent<Animal>();
                animalComp.Load(reader);
                spawnChance = animalComp.spawnChance;
                this.minProbability = minProbability;
                maxProbability = minProbability + spawnChance;
            }
        }

        public static AnimalManager Instance { get; protected set; }

        Dictionary<string, SpawnableAnimal> animalObjs = new Dictionary<string, SpawnableAnimal>();

        [SerializeField]
        Transform animalParent;
        [SerializeField]
        Transform animalInWorldParent;

        [SerializeField]
        float spawnRate = 40;
        [SerializeField]
        int spawnAmt = 20;
        [SerializeField]
        int orgSpawnAmt = 100;

        float totalAnimalProbability = 0;

        float timePassed = 0;

        System.Random updateRandom;

        void Awake() {
            Instance = this;
            updateRandom = new System.Random();
            TextAsset animals = Resources.Load<TextAsset>("Data/Animals");
            XmlTextReader reader = new XmlTextReader(new StringReader(animals.text));
            totalAnimalProbability = 0;
            if (reader.ReadToDescendant("Animals")) {
                if (reader.ReadToDescendant("Animal")) {
                    do {
                        GameObject prefab = Resources.Load<GameObject>(reader.GetAttribute("prefab"));
                        GameObject obj = GameObject.Instantiate(prefab);
                        string name = reader.GetAttribute("animalName");
                        obj.name = name;
                        obj.transform.parent = animalParent;
                        obj.SetActive(false);
                        animalObjs.Add(name, new SpawnableAnimal(obj, name, totalAnimalProbability, reader));
                        totalAnimalProbability += animalObjs[name].spawnChance;
                        Debug.Log(obj.name);
                    } while (reader.ReadToNextSibling("Animal"));
                }
            }
            if (photonView.isMine && PhotonNetwork.isMasterClient) {
                SpawnAnimalsInWorld(orgSpawnAmt);
                timePassed = 0;
            }

        }

        void Update() {
            if (photonView.isMine && PhotonNetwork.isMasterClient) {
                timePassed += Time.deltaTime;
                if (timePassed >= spawnRate) {
                    SpawnAnimalsInWorld(spawnAmt);
                    timePassed = 0;
                }
            }
        }

        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

        void SpawnAnimalsInWorld(int spawnAmt) {
            SpawnableAnimal[] list = animalObjs.Values.ToArray();
            for (int i = 0; i < spawnAmt; i++) {
                int currAnimalId = 0;
                SpawnableAnimal animal = null;
                float probability = (float)updateRandom.NextDouble() * totalAnimalProbability;
                for (int j = 0; j < list.Length; j++) {
                    if (list[j].minProbability <= probability && list[j].maxProbability >= probability) {
                        currAnimalId = j;
                        animal = list[j];
                        break;
                    }
                }
                float x = updateRandom.Next(-485, 485);
                float y = 0;
                float z = updateRandom.Next(-485, 485);
                Vector3 pos = new Vector3(x, y, z);
                //Vector3 pos = new Vector3(0.5f, y, 0.5f);
                pos.y = GameObject.FindObjectOfType<Terrain>().SampleHeight(pos) + 5;
                if (animal != null) {
                    GameObject obj = PhotonNetwork.Instantiate(animal.obj.name, pos, Quaternion.identity, 0);
                    obj.name = animal.animalName;
                    obj.transform.parent = animalInWorldParent;
                    obj.SetActive(false);
                    photonView.RPC("InstantiateAnimalInWorld", PhotonTargets.AllViaServer, currAnimalId, animalInWorldParent.childCount - 1);
                }
            }
        }

        [PunRPC]
        void InstantiateAnimalInWorld(int animalId, int objId) {
            SpawnableAnimal animal = animalObjs.Values.ToArray()[animalId];
            GameObject obj = animalInWorldParent.GetChild(objId).gameObject;
            if (animal != null && obj != null) {
                obj.SetActive(true);
                Animal animalComp = obj.GetComponent<Animal>();
                animalComp.animalName = animal.animalName;
                animalComp.Load();
            }
        }
    }
}