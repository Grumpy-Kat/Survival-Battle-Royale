using System.Xml;

namespace CrimsonPlague.Items {
    public class Clothing : ItemType {
        public float warmthAmt;

        public Clothing(Item item, XmlReader reader) : base(item, ItemSubType.Clothing, reader) { }

        public override string DisplayInfo() {
            return "Clothing \nUsed to make you warmer \nWarmth: " + warmthAmt;
        }

        public override string DisplayShortInfo() {
            return "";
        }
    }
}