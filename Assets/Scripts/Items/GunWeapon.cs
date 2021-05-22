using UnityEngine;
using System.Collections;
using System.Xml;

namespace CrimsonPlague.Items {
    public class GunWeapon : Weapon {
        public int ammo;
        public string ammoName;
        public float maxDistance;

        public AudioClip shootAudio;
        public AudioClip reloadAudio;

        public float triggerRate;
        public float timePassed = 0;

        public GameObject bullet;
        public GameObject bulletObj = null;

        public GunWeapon(Item item, XmlReader reader) : base(item, WeaponSubType.GunWeapon, reader) {
            ammo = int.Parse(reader.GetAttribute("ammo"));
            ammoName = reader.GetAttribute("ammoName");
            maxDistance = float.Parse(reader.GetAttribute("maxDistance"));
            if (reader.GetAttribute("shootAudio") != "Audio/None") {
                shootAudio = Resources.Load<AudioClip>(reader.GetAttribute("shootAudio"));
            } else {
                shootAudio = null;
            }
            if (reader.GetAttribute("reloadAudio") != "Audio/None") {
                reloadAudio = Resources.Load<AudioClip>(reader.GetAttribute("reloadAudio"));
            } else {
                reloadAudio = null;
            }
            triggerRate = float.Parse(reader.GetAttribute("triggerRate"));
            bullet = Resources.Load<GameObject>(reader.GetAttribute("bulletName"));
        }

        public override void Start() {
            base.Start();
            if (animators != null) {
                foreach (Animator animator in animators) {
                    animator.SetInteger("CurrentGunState", 1);
                }
            }
        }

        public override void Update() {
            timePassed += Time.deltaTime;
        }

        public override void End() {
            if (animators != null) {
                foreach (Animator animator in animators) {
                    animator.SetInteger("CurrentGunState", 0);
                }
            }
        }

        public override void HoldAction() {
            if (ammo == 0) {
                return;
            }
            if (timePassed < triggerRate) {
                return;
            }
            if (animators != null) {
                foreach (Animator animator in animators) {
                    animator.SetInteger("CurrentGunState", 2);
                }
            }
            timePassed = 0f;
            ammo--;
            item.src.clip = shootAudio;
            item.src.PlayOneShot(item.src.clip);
            Vector3 point;
            Quaternion orientation;
            Ray ray = item.playerMove.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Transform hit = GetTarget(ray, out point, out orientation);
            bulletObj = GameObject.Instantiate(bullet, item.transform.position, Quaternion.identity);
            bullet.transform.LookAt(ray.direction);
            Debug.DrawRay(item.transform.position, ray.direction, Color.magenta);
            bullet.GetComponent<Rigidbody>().AddForceAtPosition(ray.direction * 1500, item.transform.position);
            if (hit != null) {
                item.TakeDamage(instantDamage, hit, point, orientation);
            }
            item.StartCoroutine(StopShooting());
        }

        public override void DoOtherAction() {
            // TODO: May need to move to HoldOtherAction() and add reloadRate
            Item ammoItem = item.inventory.GetItem(ammoName);
            if (ammoItem != null) {
                foreach (ItemType type in ammoItem.types) {
                    switch (type.subType) {
                        case ItemSubType.Tool:
                            switch (((Tool)type).toolSubType) {
                                case ToolSubType.AmmoTool:
                                    if (((AmmoTool)type).ammoName == ammoName) {
                                        ammo += ((AmmoTool)type).ammoAmt;
                                        ammoItem.inventory.DestroyItem(ammoItem);
                                        item.src.clip = reloadAudio;
                                        item.src.PlayOneShot(item.src.clip);
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        IEnumerator StopShooting() {
            yield return new WaitForSeconds(1.167f);
            GameObject.Destroy(bulletObj);
            bulletObj = null;
            if (animators != null) {
                foreach (Animator animator in animators) {
                    animator.SetInteger("CurrentGunState", 1);
                }
            }
        }

        Transform GetTarget(Ray ray, out Vector3 point, out Quaternion orientation) {
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);
            Transform closestHit = null;
            float distance = 0;
            point = Vector3.zero;
            orientation = Quaternion.identity;

            // Find closest player that isn't the one shooting
            foreach (RaycastHit hit in hits) {
                if (item.GetParent(item.transform, "FG3D_Char_DeuHumans") != item.GetParent(hit.transform, "FG3D_Char_DeuHumans")) {
                    if (closestHit == null || hit.distance < distance) {
                        closestHit = hit.transform;
                        distance = hit.distance;
                        point = hit.point;
                        orientation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                    }
                }
            }

            return closestHit;
        }

        public override string DisplayInfo() {
            return "Gun \nUsed to shoot and hurt other people by pressing left-click \nCan be reloaded by pressing 'P' if you have ammo in your inventory \nDamage: " + instantDamage + " \nMaximum Reach: " + maxDistance + " \nShooting Rate: " + triggerRate + " \nAmmo: " + ammo;
        }

        public override string DisplayShortInfo() {
            return "Ammo: " + ammo;
        }
    }
}