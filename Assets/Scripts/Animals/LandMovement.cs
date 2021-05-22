using System.Xml;
using UnityEngine;

namespace CrimsonPlague.Animals {
    public class LandMovement : Movement {
        public LandMovement(Animal animal, XmlReader reader) : base(animal, MovementSubType.LandMovement, reader) { }

        public override void Update() {
            base.Update();
            float x = Mathf.Lerp(org.x, dest.x, movePercent);
            float y = 0;
            float z = Mathf.Lerp(org.z, dest.z, movePercent);
            Vector3 currPos = new Vector3(x, y, z);
            currPos.y = GameObject.FindObjectOfType<Terrain>().SampleHeight(currPos);
            //animal.transform.position = currPos;
            EndUpdate();
        }
    }
}