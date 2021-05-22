using UnityEngine;

namespace CrimsonPlague.Animals {
    public class AnimalAttack : MonoBehaviour {
        Vector3 pos;
        Quaternion rot;

        void Start() {
            pos = transform.localPosition;
            rot = transform.localRotation;
        }

        void Update() {
            transform.localPosition = pos;
            transform.localRotation = rot;
        }

        void OnCollisonEnter(Collision col) {
            Debug.Log(col.transform.name);
            Animal animal = col.gameObject.GetComponent<Animal>();
            if (animal != null) {
                transform.parent.GetComponent<Animal>().Attack(animal);
                return;
            }
            Transform player = GetParent(col.transform, "FG3D_Char_DeuHumans");
            if (player != null) {
                transform.parent.GetComponent<Animal>().Attack(player.GetComponent<CrimsonPlague.Player.Player>());
            }
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