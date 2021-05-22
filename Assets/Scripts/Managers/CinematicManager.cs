using UnityEngine;

namespace CrimsonPlague.Managers {
    public class CinematicManager : MonoBehaviour {
        [Header("Camera")]
        [SerializeField]
        Animator cam;
        [SerializeField]
        int camAnim0;
        [SerializeField]
        int camAnim1;

        [Space(10)]

        [Header("Player 0")]
        [SerializeField]
        Animator player0;
        [SerializeField]
        string player0AnimName0;
        [SerializeField]
        int player0Anim0;
        [SerializeField]
        string player0AnimName1;
        [SerializeField]
        int player0Anim1;
        [SerializeField]
        float player0Speed;

        [Space(10)]

        [Header("Player 1")]
        [SerializeField]
        Animator player1;
        [SerializeField]
        string player1AnimName0;
        [SerializeField]
        int player1Anim0;
        [SerializeField]
        string player1AnimName1;
        [SerializeField]
        int player1Anim1;
        [SerializeField]
        float player1Speed;

        void Update() {
            // Helper keys when trying to film trailer or other cinematics
            if (cam != null) {
                if (Input.GetKeyDown(KeyCode.Alpha1)) {
                    cam.SetInteger("Animation", camAnim0);
                } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                    cam.SetInteger("Animation", camAnim1);
                }
            }

            if (player0 != null) {
                player0.SetFloat("VSpeed", player0Speed);
                if (Input.GetKeyDown(KeyCode.Alpha3)) {
                    player0.SetInteger(player0AnimName0, player0Anim0);
                } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
                    player0.SetInteger(player0AnimName1, player0Anim1);
                }
            }

            if (player1 != null) {
                player1.SetFloat("VSpeed", player1Speed);
                if (Input.GetKeyDown(KeyCode.Alpha5)) {
                    player1.SetInteger(player1AnimName0, player1Anim0);
                } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
                    player1.SetInteger(player1AnimName1, player1Anim1);
                }
            }
        }
    }
}