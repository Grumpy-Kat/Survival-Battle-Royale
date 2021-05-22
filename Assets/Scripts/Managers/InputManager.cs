using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrimsonPlague.Managers {
    public class InputManager : MonoBehaviour {
        public static InputManager Instance;

        Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

        string currRebindingKey = "";

        void Awake() {
            DontDestroyOnLoad(this);
            Instance = this;
        }

        void Update() {
            if (currRebindingKey != "" && Input.anyKeyDown) {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode))) {
                    if (Input.GetKeyDown(keyCode)) {
                        keys[currRebindingKey] = keyCode;
                        break;
                    }
                }
                currRebindingKey = "";
            }
        }

        public bool GetKeyDown(string name) {
            if (keys.ContainsKey(name)) {
                return Input.GetKeyDown(keys[name]);
            }
            return false;
        }

        public bool GetKey(string name) {
            if (keys.ContainsKey(name)) {
                return Input.GetKey(keys[name]);
            }
            return false;
        }

        public bool GetKeyUp(string name) {
            if (keys.ContainsKey(name)) {
                return Input.GetKeyUp(keys[name]);
            }
            return false;
        }

        public void RebindKey(string name) {
            if (!keys.ContainsKey(name)) {
                currRebindingKey = "";
                return;
            }
            StartCoroutine(RebindKeyActual(name));
        }

        IEnumerator RebindKeyActual(string name) {
            yield return new WaitForSeconds(0.1f);
            currRebindingKey = name;
        }
    }
}