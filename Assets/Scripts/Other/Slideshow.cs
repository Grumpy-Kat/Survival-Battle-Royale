using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrimsonPlague {
    public class Slideshow : MonoBehaviour {
        [SerializeField]
        List<Sprite> backgrounds;
        int currIndex;

        [SerializeField]
        List<Image> menus;

        void Start() {
            ChangeImage();
        }

        void ChangeImage() {
            int index = 0;
            do {
                index = Random.Range((int)0, backgrounds.Count);
            } while (index == currIndex);
            currIndex = index;
            foreach (Image menu in menus) {
                menu.sprite = backgrounds[index];
            }
            Invoke("ChangeImage", 5f);
        }
    }
}