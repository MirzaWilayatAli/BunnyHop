using BunnyHop.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BunnyHop.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button playButton;

        [Header("Audio")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private float musicFadeDuration = 0.3f;

        private void Awake()
        {
            // Ensure we have a reference to the play button
            if (playButton != null)
            {
                playButton.onClick.AddListener(StartGame);
            }
        }

        private void Start()
        {
            Application.targetFrameRate = 120;
            // Play menu music when the menu loads
            SoundManager.Instance.PlayBGM(menuMusic, musicFadeDuration);
        }

        public void StartGame()
        {
            // Play game music before loading the scene
            SoundManager.Instance.PlayBGM(gameMusic, musicFadeDuration);
            SceneManager.LoadScene(1);
        }
    }
}