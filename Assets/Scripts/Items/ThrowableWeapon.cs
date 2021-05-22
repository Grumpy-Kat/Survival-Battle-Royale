using UnityEngine;
using System.Xml;

namespace CrimsonPlague.Items {
    public class ThrowableWeapon : Weapon {
        public float speed;

        public ThrowableWeapon(Item item, XmlReader reader) : base(item, WeaponSubType.ThrowableWeapon, reader) {
            speed = float.Parse(reader.GetAttribute("speed"));
        }

        public override void DoOtherAction() {
            item.isMoving = true;
            Vector3 point;
            Quaternion orientation;
            Ray ray = item.playerMove.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            GetTarget(ray, out point, out orientation);
            item.transform.LookAt(ray.direction);
            Debug.DrawRay(item.transform.position, ray.direction * speed, Color.magenta);
            item.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(ray.direction * speed, item.transform.position);
        }

        public override void OnCollisionEnter(Collision col) {
            if (item.isMoving) {
                if (item.GetParent(item.transform, "FG3D_Char_DeuHumans") != item.GetParent(col.transform, "FG3D_Char_DeuHumans")) {
                    Vector3 point = col.contacts[0].point;
                    Quaternion orientation = Quaternion.FromToRotation(Vector3.forward, col.contacts[0].normal);
                    item.TakeDamage(instantDamage, col.transform, point, orientation);
                    item.inventory.DropItem(item);
                }
            }
        }

        Transform GetTarget(Ray ray, out Vector3 point, out Quaternion orientation) {
            RaycastHit[] hits = Physics.RaycastAll(ray);
            Transform closestHit = null;
            float distance = 0;
            point = Vector3.zero;
            orientation = Quaternion.identity;

            // Find closest player that isn't the one throwing
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
            return "Throwable Weapon \nUsed to throw object at and hurt person by pressing 'P', but will take it out of your inventory \nDamage: " + instantDamage + " \nThrowing Speed: " + speed;
        }

        public override string DisplayShortInfo() {
            return "";
        }
    }
}