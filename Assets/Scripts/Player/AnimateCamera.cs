using UnityEngine;

namespace CrimsonPlague.Player {
    public class AnimateCamera : StateMachineBehaviour {
        Transform camera;
        Transform bone;

        public float smoothing = 10;

        public void Init(Animator animator) {
            camera = animator.transform.Find("Camera");
            Transform skeleton = animator.transform.GetChild(0);
            foreach (Transform bone in skeleton.GetComponentsInChildren<Transform>()) {
                if (bone.name == "head") {
                    this.bone = bone;
                }
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (camera != null && bone != null && !animator.GetBool("Swimming")) {
                camera.position = Vector3.Lerp(camera.position, bone.position + new Vector3(0, 0, 0.15f * camera.parent.parent.localScale.x), Time.deltaTime * smoothing);
                //camera.position = bone.position + new Vector3(0, 0, 0.13f);
            }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (camera != null && bone != null) {
                camera.position = Vector3.Lerp(camera.position, bone.position + new Vector3(0, 0, 0.15f * camera.parent.parent.localScale.x), Time.deltaTime * smoothing);
                camera.rotation = Quaternion.Lerp(camera.rotation, bone.rotation, Time.deltaTime * smoothing);
                //camera.position = bone.position + new Vector3(0, 0f, 0.13f);
                //camera.rotation = bone.rotation;
            }
        }
    }
}