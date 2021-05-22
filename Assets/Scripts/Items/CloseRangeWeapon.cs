using UnityEngine;
using System.Xml;
using System.Collections;

namespace CrimsonPlague.Items {
    public class CloseRangeWeapon : Weapon {
        public bool attacking = false;

        public string animationName;

        public AudioClip[] hitAudio;

        System.Random random;

        public CloseRangeWeapon(Item item, XmlReader reader) : base(item, WeaponSubType.CloseRangeWeapon, reader) {
            animationName = reader.GetAttribute("animationName");
            hitAudio = new AudioClip[2];
            hitAudio[0] = Resources.Load<AudioClip>("Audio/CloseRangeWeaponHit0");
            hitAudio[1] = Resources.Load<AudioClip>("Audio/CloseRangeWeaponHit1");
        }

        public override void Start() {
            base.Start();
            if (item.inventory != null) {
                random = new System.Random((int)PhotonNetwork.room.CustomProperties["s"] * item.inventory.photonView.owner.ID);
            }
            if (item.inventory != null && animators != null) {
                foreach (Animator animator in animators) {
                    animator.SetInteger(animationName, 1);
                }
            }
        }

        public override void DoAction() {
            int animation = random.Next(2, 5);
            if (item.inventory != null && animators != null) {
                foreach (Animator animator in animators) {
                    animator.SetInteger(animationName, animation);
                }
            }
            attacking = true;
            int audio = random.Next(0, hitAudio.Length);
            item.src.clip = hitAudio[audio];
            item.src.PlayOneShot(item.src.clip);
            item.StartCoroutine(StopHitting());
        }

        IEnumerator StopHitting() {
            yield return new WaitForSeconds(3f);
            attacking = false;
            if (item.inventory != null && animators != null) {
                foreach (Animator animator in animators) {
                    animator.SetInteger(animationName, 1);
                }
            }
        }

        public override void End() {
            if (item.inventory != null && animators != null) {
                foreach (Animator animator in animators) {
                    animator.SetInteger(animationName, 0);
                }
            }
        }

        public override void OnCollisionEnter(Collision col) {
            if (attacking) {
                if (item.GetParent(item.transform, "FG3D_Char_DeuHumans") != item.GetParent(col.transform, "FG3D_Char_DeuHumans")) {
                    Vector3 point = col.contacts[0].point;
                    Quaternion orientation = Quaternion.FromToRotation(Vector3.forward, col.contacts[0].normal);
                    item.TakeDamage(instantDamage, col.transform, point, orientation);
                }
            }
        }

        public override string DisplayInfo() {
            return "Close Range Weapon \nUsed to swing and hurt other people by pressing left-click \nDamage: " + instantDamage;
        }

        public override string DisplayShortInfo() {
            return "";
        }
    }
}