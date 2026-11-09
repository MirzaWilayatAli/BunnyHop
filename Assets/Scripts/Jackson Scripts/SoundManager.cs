using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace BunnyHop.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Settings")]
        [Range(0, 1)][SerializeField] private float defaultBGVolume = 1f;
        [Range(0, 1)][SerializeField] private float defaultSFXVolume = 1f;

        [Header("Default Audio Clips")]
        [SerializeField] private AudioClip jumpSFX;
        [SerializeField] private AudioClip collectableSound;
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameMusic;

        private AudioSource bgMusicSource;
        private AudioSource sfxSource;
        private float bgVolume;
        private float sfxVolume;
        private AudioClip currentBGM;
        private bool isFading = false;

        public float GetBGVolume() => bgVolume;
        public float GetSFXVolume() => sfxVolume;

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void InitializeAudioSources()
        {
            // Create audio sources if they don't exist
            bgMusicSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            // Configure audio sources
            bgMusicSource.playOnAwake = false;
            bgMusicSource.loop = true;
            sfxSource.playOnAwake = false;

            // Set default volumes
            bgVolume = defaultBGVolume;
            sfxVolume = defaultSFXVolume;
            SetVolume();

            // Play menu music initially
            PlayBGM(menuMusic);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Play appropriate music based on scene
            if (scene.buildIndex == 0) // Assuming 0 is menu scene
            {
                PlayBGM(menuMusic);
            }
            else // Game scene
            {
                PlayBGM(gameMusic);
            }
        }

        private void Update()
        {
            SetVolume();
        }

        private void SetVolume()
        {
            if (bgMusicSource != null)
            {
                bgMusicSource.volume = bgVolume;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
        }

        public void ChangeBGVolume(float volume)
        {
            bgVolume = Mathf.Clamp01(volume);
        }

        public void ChangeSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }

        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip, volumeScale * sfxVolume);
            }
        }

        public void PlayBGM(AudioClip clip, float fadeDuration = 0f)
        {
            if (bgMusicSource == null || clip == null || clip == currentBGM) return;

            currentBGM = clip;

            if (fadeDuration <= 0f)
            {
                // Immediate change
                bgMusicSource.clip = clip;
                bgMusicSource.Play();
            }
            else
            {
                // Fade transition
                StartCoroutine(FadeMusic(clip, fadeDuration));
            }
        }

        private IEnumerator FadeMusic(AudioClip newClip, float duration)
        {
            if (isFading) yield break;

            isFading = true;

            float startVolume = bgMusicSource.volume;
            float timer = 0f;

            // Fade out current music
            while (timer < duration && bgMusicSource.isPlaying)
            {
                timer += Time.deltaTime;
                bgMusicSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
                yield return null;
            }

            // Switch clip
            bgMusicSource.Stop();
            bgMusicSource.clip = newClip;
            bgMusicSource.Play();

            // Fade in new music
            timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                bgMusicSource.volume = Mathf.Lerp(0f, bgVolume, timer / duration);
                yield return null;
            }

            isFading = false;
        }

        // Convenience methods for default sounds
        public void PlayJumpSFX()
        {
            PlaySFX(jumpSFX);
        }

        public void PlayCollectableSound()
        {
            PlaySFX(collectableSound);
        }
    }
}