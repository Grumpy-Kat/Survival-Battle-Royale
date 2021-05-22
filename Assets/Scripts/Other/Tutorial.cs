using UnityEngine;
using UnityEngine.SceneManagement;
using CrimsonPlague.Managers;

namespace CrimsonPlague {
    public class Tutorial : MonoBehaviour {
        [SerializeField]
        GameObject startMenu;
        [SerializeField]
        GameObject startCam;

        [SerializeField]
        Player.Player player;
        [SerializeField]
        InventoryManager inventory;

        public void StartBtn() {
            startMenu.SetActive(false);
            startCam.SetActive(false);
            player.transform.parent.gameObject.SetActive(true);
            player.Init();
            inventory.gameObject.SetActive(true);
            inventory.Init();
        }

        public void ExitBtn() {
            SceneManager.LoadScene(0);
        }
    }
}