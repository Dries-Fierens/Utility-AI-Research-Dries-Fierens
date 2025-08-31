using UnityEngine;

namespace Assets.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        // To play the audio even if the object where its called gets destroyed (for example: Page)
        public void PlaySoundWhenDestroy(AudioClip sound, float volume)
        {
            if (sound != null)
            {
                GameObject audioPlayer = new();
                AudioSource tempAudioSource = audioPlayer.AddComponent<AudioSource>();
                tempAudioSource.clip = sound;
                tempAudioSource.volume = volume;
                tempAudioSource.Play();

                Destroy(audioPlayer, sound.length);
            }
        }
    }
}
