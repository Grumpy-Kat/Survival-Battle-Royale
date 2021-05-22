using UnityEngine;

namespace CrimsonPlague.Managers {
    public class AmbientAudioManager : MonoBehaviour {
        [SerializeField]
        AudioClip[] ambientAudio;
        [SerializeField]
        AudioSource src;

        // chance = 1 / (chance-1)
        // Ex: chance is 61 means chance = 1/60
        [SerializeField]
        int chance = 61;
        [SerializeField]
        float minWait = 45;
        [SerializeField]
        float maxWait = 300;
        float timePassed = 0;

        System.Random random = new System.Random();

        void Update() {
            timePassed += Time.deltaTime;
            if (timePassed >= minWait) {
                int currChance = random.Next(0, chance);
                if (currChance == 0) {
                    int audio = random.Next(0, ambientAudio.Length);
                    src.clip = ambientAudio[audio];
                    src.PlayOneShot(src.clip);
                    timePassed = 0;
                }
            }
            if (timePassed >= maxWait) {
                int audio = random.Next(0, ambientAudio.Length);
                src.clip = ambientAudio[audio];
                src.PlayOneShot(src.clip);
                timePassed = 0;
            }
        }
    }
}