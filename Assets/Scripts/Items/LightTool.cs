using System.Xml;
using UnityEngine;

namespace CrimsonPlague.Items {
    public class LightTool : Tool {
        public GameObject light;

        public LightTool(Item item, XmlReader reader) : base(item, ToolSubType.LightTool, reader) {
            light = item.transform.GetChild(0).gameObject;
        }

        public override void Start() {
            light.SetActive(false);
        }

        public override void DoOtherAction() {
            if (light != null) {
                light.SetActive(!light.activeSelf);
            }
        }

        public override string DisplayInfo() {
            return "Light \nUsed to help you see \nCan be turned on by pressing 'P'";
        }

        public override string DisplayShortInfo() {
            return (light.activeSelf ? "Light is On" : "Light is Off");
        }
    }
}