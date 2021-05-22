using System.Collections.Generic;
using UnityEngine;
using CrimsonPlague.Managers;

namespace CrimsonPlague.Player {
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Player))]
    public class PlayerMove : MonoBehaviour {
        public List<Animator> animators { get; protected set; }

        [Header("Audio")]
        AudioSource audioSource;
        [SerializeField]
        AudioClip jumpAudio;
        [SerializeField]
        AudioClip jumpLandAudio;
        [SerializeField]
        AudioClip[] stepAudio;

        MouseLook mouseLook;
        public Camera cam { get; protected set; }
        Player player;

        [Space(10)]

        [Header("Spped")]
        [SerializeField]
        float speed = 1f;
        public float itemSpeedMultiplier = 1f;
        public float speedMultiplier;
        [SerializeField]
        float runSpeedMultiplier = 2f;
        [SerializeField]
        float swimSpeed = 1.5f;

        [Space(10)]

        [Header("Footprints")]
        // This determines when should footprints appear; the upper limit for itemSpeedMultiplier (basically footprints appear when you hold heavy items or when you run)
        [SerializeField]
        float footprintThreshold = 0.85f;
        bool heavyItem = false;
        public bool isRunning { get; protected set; }
        Vector3 lastFootprint = Vector3.zero;
        [SerializeField]
        float distanceBetweenFootprints = 1;

        [Space(10)]

        [Header("Stamina")]
        [SerializeField]
        float staminaRate = 0.5f;
        [SerializeField]
        float jumpStaminaRate = 10f;

        public void Init() {
            animators = new List<Animator>();
            animators.Add(GetComponent<Animator>());
            animators.Add(GameObject.FindGameObjectWithTag("Clothes").GetComponent<Animator>());
            animators[0].GetBehaviour<AnimateCamera>().Init(animators[0]);
            audioSource = GetComponent<AudioSource>();
            cam = transform.Find("Camera").GetComponent<Camera>();
            mouseLook = new MouseLook();
            mouseLook.Init(transform, cam.transform);
            player = transform.GetComponent<Player>();
            isRunning = false;
        }

        public void Move() {
            if (!player.inventory.panel.activeInHierarchy && (!player.useMap || !MapManager.Instance.map.activeInHierarchy)) {
                mouseLook.LookRotation(transform, cam.transform);

                if (player.GetStamina() >= staminaRate) {
                    if (Input.GetButtonDown("Run") && !isRunning) {
                        speed *= runSpeedMultiplier;
                        swimSpeed *= runSpeedMultiplier;
                        isRunning = true;
                    } else if (Input.GetButtonUp("Run") && isRunning) {
                        speed /= runSpeedMultiplier;
                        swimSpeed /= runSpeedMultiplier;
                        isRunning = false;
                    }
                    if (Input.GetButton("Run") && isRunning) {
                        player.AddStamina(-staminaRate);
                    }
                } else if (isRunning) {
                    speed /= runSpeedMultiplier;
                    swimSpeed /= runSpeedMultiplier;
                    isRunning = false;
                }

                if (itemSpeedMultiplier <= footprintThreshold) {
                    heavyItem = true;
                } else {
                    heavyItem = false;
                }

                foreach (Animator animator in animators) {
                    animator.SetFloat("VSpeed", Input.GetAxis("Vertical") * speed * itemSpeedMultiplier * speedMultiplier);
                    animator.SetFloat("HSpeed", Input.GetAxis("Horizontal") * speed * itemSpeedMultiplier * speedMultiplier);
                    animator.SetFloat("SwimSpeed", Input.GetAxis("Vertical") * swimSpeed * itemSpeedMultiplier * speedMultiplier);
                }

                if (Input.GetAxis("Vertical") < -0.1f || Input.GetAxis("Vertical") > 0.1f || Input.GetAxis("Horizontal") < -0.1f || Input.GetAxis("Horizontal") > 0.1f) {
                    if (!Input.GetButtonDown("Jump")) {
                        if (!audioSource.isPlaying) {
                            int n = Random.Range(1, stepAudio.Length);
                            audioSource.clip = stepAudio[n];
                            audioSource.PlayOneShot(audioSource.clip);
                            stepAudio[n] = stepAudio[0];
                            stepAudio[0] = audioSource.clip;
                        }
                    }
                }

                if (player.GetStamina() >= jumpStaminaRate) {
                    if (Input.GetButtonDown("Jump")) {
                        foreach (Animator animator in animators) {
                            animator.SetBool("Jumping", true);
                        }
                        if (!animators[0].GetBool("Swimming")) {
                            if (!audioSource.isPlaying) {
                                audioSource.clip = jumpAudio;
                                audioSource.Play();
                            }
                            player.AddStamina(-jumpStaminaRate);
                            Invoke("StopJumping", 0.1f);
                        }
                    }
                }

                if (animators[0].GetBool("Swimming") && animators[0].GetBool("Jumping")) {
                    Vector3 newPosition = transform.position + new Vector3(0, swimSpeed / 2f, 0);
                    newPosition = new Vector3(newPosition.x, Mathf.Clamp(newPosition.y, -100, gameObject.GetComponent<Player>().waterLevel - 0.3f), newPosition.z);
                    transform.position = newPosition;
                }

                Transform skeleton = animators[0].transform.GetChild(0);
                Transform clothesSkeleton = animators[1].transform.GetChild(0);
                animators[1].transform.position = animators[0].transform.position;
                animators[1].transform.rotation = animators[0].transform.rotation;

                foreach (Transform bone in skeleton.GetComponentsInChildren<Transform>()) {
                    Transform clothesBone = clothesSkeleton.Find(bone.name);
                    if (clothesBone != null) {
                        clothesBone.position = bone.position;
                        clothesBone.rotation = bone.rotation;
                        continue;
                    }
                }

                if (!player.singlePlayer && (heavyItem || isRunning) && !animators[0].GetBool("Jumping") && !animators[0].GetBool("Swimming")) {
                    if (Vector3.Distance(lastFootprint, skeleton.position) >= distanceBetweenFootprints) {
                        PhotonNetwork.Instantiate("Footprints", skeleton.position + new Vector3(0, 0.4f, 0), skeleton.rotation, 0);
                        lastFootprint = skeleton.position;
                    }
                }
            }
        }
        void StopJumping() {
            foreach (Animator animator in animators) {
                animator.SetBool("Jumping", false);
            }
            audioSource.clip = jumpLandAudio;
            audioSource.Play();
        }
    }
}