using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Events.SceneManager
{
    public class SceneManager : MonoBehaviour
    {
        private bool _eventsHooked;

        // Events visible within the Inspector
        public UnityEvent onRestart;
        public UnityEvent onGameOver;
        public UnityEvent<int> onScoreChanged;

        [Tooltip("Set this to the text you want to update with the score")]
        [SerializeField] private TMP_Text scoreText;

        [SerializeField] private AudioClip introMusic;
        [SerializeField] private AudioClip gameMusic;


        private void HookEvents()
        {
            // check if there is a valid GameManager instance, this should realistically always be the case, but like - you never know right?
            // also check if events are already hooked, this is a measure in place because Unity's execution order is sometimes uncooperative.
            if (GameManager.Instance && _eventsHooked == false)
            {
                Logger.Log(this, "Event hooked!");
                GameManager.Instance.OnRestart += OnRestartTrigger;
                GameManager.Instance.OnGameOver += OnGameOverTrigger;
                GameManager.Instance.OnScoreChanged += OnScoreChangedTrigger;
                _eventsHooked = true;
            }
            else
            {
                Logger.Log(this, "Events failed to hook.");
            }
        }

        // ----- This part edited by Wilayat -----
        private IEnumerator WaitForGameManager()
        {
            while (GameManager.Instance == null)
                yield return null;

            HookEvents();
        }

        private void Start()
        {
            StartCoroutine(WaitForGameManager());
        }
        // ------ end of Wilayat's edited part ------


        private void OnScoreChangedTrigger(int obj)
        {
            // Call whatever has hooked into the Action
            onScoreChanged?.Invoke(obj);
            scoreText.SetText($"{obj}");
        }

        private void OnEnable()
        {
            // Precautionary measure, in case somehow OnDisable ever triggers and
            // the object is re-enabled.
            // This should never happen, but I guess we should probably be prepared.
            // What if a cosmic bitflip ever happens?!!!??! (joke)
            if (GameManager.Instance)
            {
                HookEvents();
            }
        }

        private void OnDisable()
        {
            // Checks if valid GameManager instance and then unhooks everything
            if (GameManager.Instance)
            {
                Logger.Log(this, "Events unhooked.");
                GameManager.Instance.OnRestart -= OnRestartTrigger;
                GameManager.Instance.OnGameOver -= OnGameOverTrigger;
                GameManager.Instance.OnScoreChanged -= OnScoreChangedTrigger;
                _eventsHooked = false;
            }
        }

        // To use OnRestart/OnGameOver in scene:
        //
        // create a reference to the SceneManager in your script
        // (eg. sceneManager)
        //
        // sceneManager.onRestart += Function;
        //
        // you should unhook it if your object is off, and rehook it if it is on.

        private void OnGameOverTrigger()
        {
            // Scene specific GameOver triggers in scene should hook OnGameOver
            onGameOver?.Invoke();
        }

        private void OnRestartTrigger()
        {
            // Scene specific Restart triggers in scene should hook OnRestart
            onRestart?.Invoke();
        }

        // This is for the button _in scene_.
        public void RestartButton()
        {
            if (GameManager.Instance && GameManager.Instance.gameOver)
            {
                GameManager.Instance.RestartGame();
            }
        }

        public void LoadMainMenu()
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.RestartGame();
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
        public void LoadGameScene()
        {
            GameManager.Instance.RestartGame();
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}
