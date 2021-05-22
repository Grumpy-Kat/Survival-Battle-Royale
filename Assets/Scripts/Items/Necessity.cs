using System.Xml;
using UnityEngine;
using CrimsonPlague.Managers;

namespace CrimsonPlague.Items {
    public class Necessity : ItemType {
        public Necessity(Item item, XmlReader reader) : base(item, ItemSubType.Necessity, reader) { }

        public override void Start() {
            Transform canvas = GameObject.Find("Canvas").transform;
            switch (item.itemName) {
                case "Map":
                    canvas.Find("OpenMapButton").gameObject.SetActive(true);
                    canvas.Find("Map").gameObject.SetActive(true);
                    if (MapManager.Instance != null) {
                        MapManager.Instance.isUseable = true;
                    }
                    break;
                default:
                    Debug.Log(item.itemName);
                    break;
            }
        }

        public override void End() {
            Transform canvas = GameObject.Find("Canvas").transform;
            switch (item.itemName) {
                case "Map":
                    canvas.Find("OpenMapButton").gameObject.SetActive(false);
                    canvas.Find("Map").gameObject.SetActive(false);
                    canvas.Find("FullScreenMap").gameObject.SetActive(false);
                    if (MapManager.Instance != null) {
                        MapManager.Instance.isUseable = false;
                    }
                    break;
                default:
                    Debug.Log(item.itemName);
                    break;
            }
        }

        public override string DisplayInfo() {
            return "";
        }

        public override string DisplayShortInfo() {
            return "";
        }
    }
}