using System.Xml;
using UnityEngine;

namespace CrimsonPlague.Animals {
    public enum MovementSubType { Default, LandMovement, AirMovement }

    public class Movement {
        public Animal animal;

        public MovementSubType subType;

        public float speed;
        public bool isBeingAttacked = false;
        public float attackSpeedMultiplier;

        protected bool isPaused = false;
        protected float timeSincePaused = 0;
        protected float timePaused = 0;

        public Vector3 org;
        public Vector3 dest;

        protected float movePercent; // Ranges 0 - 1 and changes as the character moves from org to dest

        public Movement(Animal animal, MovementSubType subType, XmlReader reader) {
            this.animal = animal;
            this.subType = subType;
            speed = float.Parse(reader.GetAttribute("speed"));
            attackSpeedMultiplier = float.Parse(reader.GetAttribute("attackSpeedMultiplier"));
        }

        public virtual void Start() {
            org = animal.transform.position;
            dest = animal.transform.position;
        }

        public virtual void Update() {
            if (!isBeingAttacked && !isPaused) {
                System.Random random = new System.Random();
                if (timeSincePaused > 15) {
                    if (random.NextDouble() > 0.1f) {
                        isPaused = true;
                        timePaused = random.Next(2, 15);
                        return;
                    }
                } else if (timeSincePaused > 60) {
                    isPaused = true;
                    timePaused = random.Next(2, 15);
                    // TODO: make sure animals have landed when paused
                    return;
                }
            }
            
            if (isPaused && !isBeingAttacked) {
                timePaused -= Time.deltaTime;
                if (timePaused > 0) {
                    return;
                }
            }

            float dist = Vector3.Distance(org, dest);
            Vector3 dir = dest - animal.transform.position;
            Quaternion toRot = Quaternion.FromToRotation(animal.transform.forward, dir);
            animal.transform.rotation = Quaternion.Lerp(animal.transform.rotation, toRot, 0.1f * Time.deltaTime);
            float[] rotList = new float[] { toRot.eulerAngles.x, toRot.eulerAngles.y, toRot.eulerAngles.z };
            int largest = 0;
            int second = 0;
            float total = 0;
            for (int i = 0; i < rotList.Length; i++) {
                if (rotList[i] >= rotList[largest]) {
                    second = largest;
                    largest = i;
                } else if (rotList[i] >= rotList[second]) {
                    second = i;
                }
                total += rotList[i];
            }
            Vector3 childRot = new Vector3(0, 0, 0);
            switch (largest) {
                case 1:
                    childRot.y = 20;
                    break;
                case 2:
                    childRot.z = 20;
                    break;
            }
            if (second == 0) {
                second = largest;
                childRot.x = -20;
            }
            switch (second) {
                case 1:
                    childRot.y = total;
                    break;
                case 2:
                    childRot.z = total;
                    break;
            }
            animal.transform.GetChild(0).localEulerAngles = childRot;
            
            // Calculating the distance and percentage moved this frame
            float moveThisFrame = Time.deltaTime * speed;
            if (isBeingAttacked) {
                moveThisFrame *= attackSpeedMultiplier;
            }
            float percThisFrame = moveThisFrame / dist;
            movePercent += percThisFrame;
            animal.animator.SetBool("isRunning", isBeingAttacked);
            animal.animator.SetBool("isWalking", (movePercent > 0));
        }

        protected void EndUpdate() {
            if (movePercent >= 1 && !animal.response.isAttacking) {
                animal.animator.SetBool("isRunning", false);
                animal.animator.SetBool("isWalking", false);

                // Reached target destination
                GeneratePos();
                movePercent = 0;
            }
        }

        public virtual void GeneratePos() {
            org = animal.transform.position;
            org.y = 0;
            System.Random random = new System.Random();
            float x = random.Next(-485, 485);
            float z = random.Next(-485, 485);
            dest = new Vector3(x, 0, z);
        }
    }
}