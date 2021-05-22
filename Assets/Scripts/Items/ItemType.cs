using System.Xml;
using System.Collections.Generic;
using UnityEngine;

namespace CrimsonPlague.Items {
    public enum ItemSubType { Default, Weapon, Clothing, Food, Tool, Medicine, Necessity }

    public class ItemType {
        public Item item;

        public ItemSubType subType;

        public List<Animator> animators;

        public ItemType(Item item, ItemSubType subType, XmlReader reader) {
            this.item = item;
            this.subType = subType;
        }

        public virtual void Start() {
            if (item.playerMove != null) {
                animators = item.playerMove.animators;
            }
            if (animators == null) {
                animators = new List<Animator>();
            }
        }

        public virtual void Update() { }

        public virtual void End() { }

        // Item types have a primary action that can require the button to be held or simply pressed
        public virtual void DoAction() { }
        public virtual void HoldAction() { }
        public virtual void FinishAction() { }

        // Item types have a secondary action that can require the button to be held or simply pressed
        public virtual void DoOtherAction() { }
        public virtual void HoldOtherAction() { }
        public virtual void FinishOtherAction() { }

        public virtual void OnCollisionEnter(Collision col) { }

        public virtual string DisplayInfo() {
            return "";
        }

        public virtual string DisplayShortInfo() {
            return "";
        }
    }
}