using System.Xml;

namespace CrimsonPlague.Items {
    public enum WeaponSubType { Default, GunWeapon, CloseRangeWeapon, ThrowableWeapon }

    public class Weapon : ItemType {
        public float instantDamage;
        public float slowDamage;
        public float damageRate;

        public WeaponSubType weaponSubType;

        public Weapon(Item item, WeaponSubType weaponSubType, XmlReader reader) : base(item, ItemSubType.Weapon, reader) {
            instantDamage = float.Parse(reader.GetAttribute("instantDamage"));
            this.weaponSubType = weaponSubType;
        }

        public override string DisplayInfo() {
            return "Weapon \nUsed to hurt other people \nDamage: " + instantDamage;
        }

        public override string DisplayShortInfo() {
            return "";
        }
    }
}