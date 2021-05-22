using System.Xml;

namespace CrimsonPlague.Items {
    public class AmmoTool : Tool {
        public int minAmmo;
        public int maxAmmo;

        public int ammoAmt;

        public string ammoName;

        public AmmoTool(Item item, XmlReader reader) : base(item, ToolSubType.AmmoTool, reader) {
            minAmmo = int.Parse(reader.GetAttribute("minAmmo"));
            maxAmmo = int.Parse(reader.GetAttribute("maxAmmo"));
            System.Random random;
            if (PhotonNetwork.room == null) {
                random = new System.Random();
            } else {
                if (item.inventory == null) {
                    random = new System.Random((int)PhotonNetwork.room.CustomProperties["s"]);
                } else {
                    random = new System.Random((int)PhotonNetwork.room.CustomProperties["s"] * item.inventory.photonView.owner.ID);
                }
            }
            ammoAmt = random.Next(minAmmo, maxAmmo + 1);
            ammoName = reader.GetAttribute("ammoName");
        }

        public override string DisplayInfo() {
            return "Ammo \nUsed to reload your gun or bow (depending on the type) \nAmount: " + ammoAmt;
        }

        public override string DisplayShortInfo() {
            return "";
        }
    }
}