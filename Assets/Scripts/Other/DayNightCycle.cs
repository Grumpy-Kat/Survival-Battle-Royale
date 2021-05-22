using UnityEngine;

namespace CrimsonPlague {
    public class DayNightCycle : MonoBehaviour {
        [SerializeField]
        float speed = 6;
        float cycle;

        void Start() {
            cycle = 0.1f / speed * -1;
        }

        void Update() {
            transform.Rotate(0, 0, cycle, Space.World);
        }
    }
}