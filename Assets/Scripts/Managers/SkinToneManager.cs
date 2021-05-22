using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrimsonPlague.Managers {
    public class SkinToneManager : MonoBehaviour {
        public int currSelectedIndex { get; protected set; }

        Dictionary<int, Text> inventoryObjs = new Dictionary<int, Text>();

        public GameObject itemPrefab;
        [SerializeField]
        ScrollRect scroll;

        Button currSelectedBtn = null;

        // Must be the one with the Player component on it
        [SerializeField]
        GameObject player;

        void Start() {
            for (int i = 0; i < CustomizeManager.Instance.skinTones.Count; i++) {
                AddOption(i);
            }
            UpdatePicker();
        }

        void Update() {
            if (Input.GetButtonDown("NavigateInventory") && Input.GetAxis("NavigateInventory") > 0.1f) {
                currSelectedIndex--;
                if (currSelectedIndex < 0) {
                    currSelectedIndex = CustomizeManager.Instance.skinTones.Count - 1;
                } else if (currSelectedIndex > CustomizeManager.Instance.skinTones.Count - 1) {
                    currSelectedIndex = 0;
                }
                UpdatePicker();
            } else if (Input.GetButtonDown("NavigateInventory") && Input.GetAxis("NavigateInventory") < -0.1f) {
                currSelectedIndex++;
                if (currSelectedIndex < 0) {
                    currSelectedIndex = CustomizeManager.Instance.skinTones.Count - 1;
                } else if (currSelectedIndex > CustomizeManager.Instance.skinTones.Count - 1) {
                    currSelectedIndex = 0;
                }
                UpdatePicker();
            }
        }

        void UpdatePicker() {
            inventoryObjs[currSelectedIndex].GetComponentInParent<Button>().onClick.Invoke();
            scroll.verticalScrollbar.value = 1 - ((float)currSelectedIndex / (inventoryObjs.Count - 1));
        }

        public void AddOption(int index) {
            // TODO: Maybe add names?
            GameObject obj = GameObject.Instantiate(itemPrefab, new Vector3(250, (40 * inventoryObjs.Keys.Count) + 150, 0), itemPrefab.transform.rotation);
            obj.transform.SetParent(transform);
            obj.GetComponent<Button>().onClick.AddListener(() => { SelectOption(index, obj.GetComponent<Button>()); });
            obj.transform.GetChild(0).GetComponent<Text>().text = index.ToString();
            inventoryObjs.Add(index, obj.transform.GetChild(0).GetComponent<Text>());
        }

        public void SelectOption(int index, Button button) {
            if (currSelectedBtn != null) {
                currSelectedBtn.interactable = true;
            }
            if (button != null) {
                button.interactable = false;
            }
            currSelectedBtn = button;
            currSelectedIndex = index;
            
            // Sets setting to preserve skin tone when changing scenes and through networking
            CustomizeManager.Instance.WearSkinTone(index, player);
        }
    }
}