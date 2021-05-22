using System.Xml;
using UnityEngine;

namespace CrimsonPlague.Animals {
    public class ExtraAggressiveResponse : Response {
        Player.Player playerTarget;
        Animal animalTarget;

        float huntingDistance;
        float damage;

        public ExtraAggressiveResponse(Animal animal, XmlReader reader) : base(animal, ResponseSubType.ExtraAggressiveResponse, reader) {
            huntingDistance = float.Parse(reader.GetAttribute("huntingDistance"));
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
            } else if (playerTarget == null && animalTarget == null) {
                isAttacking = false;
                Player.Player[] players = GameObject.FindObjectsOfType<CrimsonPlague.Player.Player>();

                foreach (Player.Player player in players) {
                    if (Vector3.Distance(player.transform.position, animal.transform.position) <= huntingDistance) {
                        playerTarget = player;
                        break;
                    }
                }
            }
        }

        public override void TakeDamage(Player.Player attacker) {
            base.TakeDamage(attacker);
            if (attacker != null) {
                playerTarget = attacker;
                isAttacking = true;
            }
        }

        public override void TakeDamage(Animal attacker) {
            base.TakeDamage(attacker);
            if (attacker != null) {
                animalTarget = attacker;
                isAttacking = true;
            }
        }

        public override void Attack(CrimsonPlague.Player.Player prey) {
            if (prey != null) {
                prey.TakeDamage(damage);
            }
        }

        public override void Attack(Animal prey) {
            if (prey != null) {
                prey.TakeDamage(damage, animal);
            }
        }
    }
}