using UnityEngine;
using UnityEngine.UI;

namespace CrimsonPlague.Network {
    public class Options : MonoBehaviour {
        Button resObj;
        Button graphicsObj;

        void Awake() {
            DontDestroyOnLoad(this);
            LoadSettings();
        }

        void LoadSettings() { /* TODO */ }

        void SaveSettings() { /* TODO */ }

        public void SetResolution(GameObject obj) {
            resObj.interactable = true;
            int width = int.Parse(obj.name.Split('x')[0]);
            int height = int.Parse(obj.name.Split('x')[1]);
            Button newObj = obj.GetComponent<Button>();
            resObj = newObj;
            resObj.interactable = false;
            Screen.SetResolution(width, height, Screen.fullScreen);
        }

        public void SetFullScreen(Toggle obj) {
            Screen.fullScreen = obj.isOn;
        }

        public void SetGraphicsQuality(GameObject obj) {
            graphicsObj.interactable = true;
            int level = int.Parse(obj.name);
            Button newObj = obj.GetComponent<Button>();
            graphicsObj = newObj;
            graphicsObj.interactable = false;
            QualitySettings.SetQualityLevel(level, true);
        }

        public void SetMasterVolume(Slider obj) {
            AudioListener.volume = obj.value;
        }

        public void SetMusicVolume(Slider obj) { /* TODO */ }

        public void SetEffectsVolume(Slider obj) { /* TODO */ }
    }
}