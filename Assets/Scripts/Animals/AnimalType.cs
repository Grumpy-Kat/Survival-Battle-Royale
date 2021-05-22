using System.Xml;
using UnityEngine;

namespace CrimsonPlague.Animals {
    public enum AnimalSubType { Default, AirMovement, LandMovement, Aggressive, Passive }

    public class AnimalType : MonoBehaviour {
        public Animal item;

        public AnimalSubType subType;

        public Animator animator;

        public AnimalType(Animal item, AnimalSubType subType, XmlReader reader) {
            this.item = item;
            this.subType = subType;
        }

        public virtual void Start() {
            animator = item.gameObject.GetComponent<Animator>();
        }

        public virtual void Update() { }

        public virtual void OnCollisionEnter(Collision col) { }

        public virtual void TakeDamage() { }
    }
}