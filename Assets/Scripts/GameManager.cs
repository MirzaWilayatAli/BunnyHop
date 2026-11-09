using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Scroller scroller;
    [SerializeField] private PlayerController player;
    [SerializeField] private int score;
    [Tooltip("State of game")]
    public bool gameOver;

    public bool shouldLog;
    
    public Action OnGameOver;
    public Action OnRestart;
    public Action<int> OnScoreChanged;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    // Holds active instances of scroller and player
    public void SetPlayer(PlayerController player) => this.player = player;
    public void SetScroller(Scroller scroller) => this.scroller = scroller;
    
    // Score handling
    public int GetScore() => score;
    public void IncreaseScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }
    public void SetScore(int value)
    {
        score = value;
        OnScoreChanged?.Invoke(score);
    }

    // Restart logic
    public void RestartGame()
    {
        // Reset score, player and scroller and call OnRestart.
        SetScore(0);
        player.ResetPlayer();
        gameOver = false;
        OnRestart?.Invoke();
    }
    public void GameOver()
    {
        player.isAlive = false;
        gameOver = true;
        OnGameOver?.Invoke();
        LeaderboardManager.Instance.SubmitScoreVoid(score);
    }
}
