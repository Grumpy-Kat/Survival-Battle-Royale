using System.Xml;
using UnityEngine;

namespace CrimsonPlague.Animals {
    public class AirMovement : Movement {
        public AirMovement(Animal animal, XmlReader reader) : base(animal, MovementSubType.AirMovement, reader) { }

        public override void Update() {
            base.Update();
            float x = Mathf.Lerp(org.x, dest.x, movePercent);
            System.Random random = new System.Random();
            float y = animal.transform.position.y + (((float)random.NextDouble() / 4) - 0.125f);
            float z = Mathf.Lerp(org.z, dest.z, movePercent);
            Vector3 currPos = new Vector3(x, y, z);
            float terrainY = GameObject.FindObjectOfType<Terrain>().SampleHeight(currPos);
            if (terrainY + 7 >= currPos.y) {
                currPos.y = terrainY + 7.1f;
            }
            animal.transform.position = currPos;
            EndUpdate();
        }
    }
}