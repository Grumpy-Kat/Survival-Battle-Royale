using System.Collections.Generic;
using UnityEngine;

namespace CrimsonPlague.Managers {
    public class CustomizeManager : MonoBehaviour {
        public static CustomizeManager Instance;

        public List<Material> skinTones = new List<Material>();
        public List<Material> outfits = new List<Material>();

        void Awake() {
            DontDestroyOnLoad(this);
            Instance = this;
        }

        public void WearSkinTone(int index, GameObject player) {
            if (player != null) {
                Transform parent = player.transform.GetChild(0);

                for (int i = 0; i < 3; i++) {
                    SkinnedMeshRenderer renderer = parent.GetChild(i).GetComponent<SkinnedMeshRenderer>();
                    Material[] materials = renderer.materials;
                    // Hard coded materials that require the skin tone to be set
                    materials[1] = skinTones[index];
                    materials[3] = skinTones[index];
                    materials[4] = skinTones[index];
                    materials[5] = skinTones[index];
                    materials[6] = skinTones[index];
                    materials[8] = skinTones[index];
                    renderer.materials = materials;
                }

                for (int i = 3; i < 5; i++) {
                    SkinnedMeshRenderer renderer = parent.GetChild(i).GetComponent<SkinnedMeshRenderer>();
                    Material[] materials = renderer.materials;
                    // Hard coded materials that require the skin tone to be set
                    materials[1] = skinTones[index];
                    materials[2] = skinTones[index];
                    materials[3] = skinTones[index];
                    materials[4] = skinTones[index];
                    materials[5] = skinTones[index];
                    renderer.materials = materials;
                }
            }
        }

        public void WearOutfit(int index, GameObject player) {
            if (player != null) {
                Transform parent = player.transform.parent.GetChild(1).GetChild(0);
                SkinnedMeshRenderer renderer = parent.GetChild(0).GetComponent<SkinnedMeshRenderer>();
                Material[] materials = renderer.materials;
                // Hard coded materials that require the outfit tone to be set
                materials[0] = outfits[index];
                materials[2] = outfits[index];
                renderer.materials = materials;
            }
        }
    }
}