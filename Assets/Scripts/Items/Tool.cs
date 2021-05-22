using System.Xml;

namespace CrimsonPlague.Items {
    public enum ToolSubType { Default, LightTool, AmmoTool }

    public class Tool : ItemType {
        public ToolSubType toolSubType;

        public Tool(Item item, ToolSubType toolSubType, XmlReader reader) : base(item, ItemSubType.Tool, reader) {
            this.toolSubType = toolSubType;
        }

        public override string DisplayInfo() {
            return "Tool \nUsed a misc object";
        }

        public override string DisplayShortInfo() {
            return "";
        }
    }
}