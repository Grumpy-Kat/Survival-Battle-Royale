using System.Xml;
using UnityEngine;

namespace CrimsonPlague.Items {
    public class Medicine : ItemType {
        public float replenishAmt;
        public int woundsHealed;
        public float timeToEat;

        public float timePassed;

        public Medicine(Item item, XmlReader reader) : base(item, ItemSubType.Medicine, reader) {
            replenishAmt = float.Parse(reader.GetAttribute("replenishAmt"));
            woundsHealed = int.Parse(reader.GetAttribute("woundsHealed"));
            timeToEat = float.Parse(reader.GetAttribute("timeToEat"));
        }

        public override void Update() {
            timePassed += Time.deltaTime;
        }

        public override void DoOtherAction() {
            timePassed = 0;
        }

        public override void HoldOtherAction() {
            if (timePassed < timeToEat) {
                return;
            }
            item.inventory.player.AddHealth(replenishAmt);
            item.inventory.player.HealDamage(woundsHealed);
            item.inventory.DestroyItem(item);
        }

        public override string DisplayInfo() {
            return "Medicine \nUsed to stop bleeding and add health by pressing 'P' \nStops: " + woundsHealed + " bleeding \nHealth: " + replenishAmt;
        }

        public override string DisplayShortInfo() {
            return "";
        }
    }
}