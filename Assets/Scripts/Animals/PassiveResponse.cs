using System.Xml;

namespace CrimsonPlague.Animals {
    public class PassiveResponse : Response {
        public PassiveResponse(Animal animal, XmlReader reader) : base(animal, ResponseSubType.PassiveResponse, reader) { }

        public override void TakeDamage(CrimsonPlague.Player.Player attacker) {
            base.TakeDamage(attacker);
            animal.movement.GeneratePos();
        }

        public override void TakeDamage(Animal attacker) {
            base.TakeDamage(attacker);
            animal.movement.GeneratePos();
        }
    }
}