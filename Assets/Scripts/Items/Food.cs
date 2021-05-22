using System.Xml;
using UnityEngine;

namespace CrimsonPlague.Items {
    public class Food : ItemType {
        public bool hunger;
        public float replenishAmt;
        public float timeToEat;

        public bool isPoisonous;
        public float poisonAmt;

        public float spoilRate;
        public bool isSpoiled;

        public float timePassedTotal = 0;
        public float timePassed = 0;

        public Food(Item item, XmlReader reader) : base(item, ItemSubType.Food, reader) {
            hunger = bool.Parse(reader.GetAttribute("hunger"));
            replenishAmt = float.Parse(reader.GetAttribute("replenishAmt"));
            timeToEat = float.Parse(reader.GetAttribute("timeToEat"));
            isPoisonous = bool.Parse(reader.GetAttribute("isPoisonous"));
            poisonAmt = float.Parse(reader.GetAttribute("poisonAmt"));
            spoilRate = float.Parse(reader.GetAttribute("spoilRate"));
            isSpoiled = bool.Parse(reader.GetAttribute("isSpoiled"));
        }

        public override void Update() {
            timePassedTotal += Time.deltaTime;
            if (timePassedTotal >= spoilRate) {
                isSpoiled = true;
            }
            timePassed += Time.deltaTime;
        }

        public override void DoOtherAction() {
            timePassed = 0;
        }

        public override void HoldOtherAction() {
            if (timePassed < timeToEat) {
                return;
            }
            if (isPoisonous || isSpoiled) {
                item.inventory.player.AddHealth(-poisonAmt);
                if (hunger) {
                    item.inventory.player.AddHunger(replenishAmt / 2f);
                } else {
                    item.inventory.player.AddThirst(replenishAmt / 2f);
                }
            } else {
                if (hunger) {
                    item.inventory.player.AddHunger(replenishAmt);
                } else {
                    item.inventory.player.AddThirst(replenishAmt);
                }
            }
            item.inventory.DestroyItem(item);
        }

        public override string DisplayInfo() {
            return "Food \nUsed to provide " + (hunger ? "hunger" : "thirst") + " by pressing 'P' \nReplenishes: " + replenishAmt + " \n" + (isSpoiled ? "Spoiled" : "Spoils in: " + (spoilRate / 1000) + "seconds") + (isPoisonous ? " \nPoisonous" : "");
        }

        public override string DisplayShortInfo() {
            return "";
        }
    }
}