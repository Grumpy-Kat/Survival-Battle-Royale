using System.Xml;
using UnityEngine;

namespace CrimsonPlague.Animals {
    public enum ResponseSubType { Default, ExtraAggressiveResponse, AggressiveResponse, PassiveResponse }

    public class Response {
        public Animal animal;

        public ResponseSubType subType;

        protected float timeSinceAttack;
        //how long does the animal remember the last attack
        public float memoryTime;

        public bool isAttacking = false;

        public Response(Animal animal, ResponseSubType subType, XmlReader reader) {
            this.animal = animal;
            this.subType = subType;
            memoryTime = float.Parse(reader.GetAttribute("memoryTime"));
        }

        public virtual void Start() { }

        public virtual void Update() {
            timeSinceAttack += Time.deltaTime;
            if (timeSinceAttack >= memoryTime) {
                animal.movement.isBeingAttacked = false;
            }
        }

        public virtual void TakeDamage(CrimsonPlague.Player.Player attacker) {
            timeSinceAttack = 0;
            animal.movement.isBeingAttacked = true;
        }

        public virtual void TakeDamage(Animal attacker) {
            timeSinceAttack = 0;
            animal.movement.isBeingAttacked = true;
        }

        public virtual void Attack(CrimsonPlague.Player.Player prey) { }
        public virtual void Attack(Animal prey) { }
    }
}