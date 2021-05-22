using System.Xml;
using UnityEngine;

namespace CrimsonPlague.Animals {
    public class AggressiveResponse : Response {
        CrimsonPlague.Player.Player playerTarget;
        Animal animalTarget;

        float damage;

        public AggressiveResponse(Animal animal, XmlReader reader) : base(animal, ResponseSubType.AggressiveResponse, reader) {
            damage = float.Parse(reader.GetAttribute("damage"));
        }


        public override void Update() {
            base.Update();
            if (timeSinceAttack < memoryTime && (playerTarget != null || animalTarget != null)) {
                animal.movement.org = animal.transform.position;
                if (playerTarget != null) {
                    animal.movement.dest = playerTarget.transform.position;
                } else {
                    animal.movement.dest = animalTarget.transform.position;
                }
                isAttacking = true;
            } else {
                isAttacking = false;
            }
            if (playerTarget != null) {
                if (Vector3.Distance(playerTarget.transform.position, animal.transform.position) <= 1) {
                    //TODO: ATTACK!
                }
            }
        }

        public override void TakeDamage(CrimsonPlague.Player.Player attacker) {
            base.TakeDamage(attacker);
            if (attacker != null) {
                playerTarget = attacker;
            }
        }

        public override void TakeDamage(Animal attacker) {
            base.TakeDamage(attacker);
            if (attacker != null) {
                animalTarget = attacker;
            }
        }

        public override void Attack(CrimsonPlague.Player.Player prey) {
            if (isAttacking && prey != null) {
                prey.TakeDamage(damage);
            }
        }

        public override void Attack(Animal prey) {
            if (isAttacking && animalTarget == prey && prey != null) {
                prey.TakeDamage(damage, animal);
            }
        }
    }
}