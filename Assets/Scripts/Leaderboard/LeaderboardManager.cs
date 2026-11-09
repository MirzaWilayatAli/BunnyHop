using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard UI")]
    [SerializeField] private Transform leaderboardContent;
    [SerializeField] private PlayerScoreLeaderboardItem leaderboardItemPrefab;
    
    public static LeaderboardManager Instance;

    [SerializeField] private string leaderboardId = "Bunny_Hop_High_Score_Leaderboard";

    private const string NameKey = "PlayerName";
    private Task _initTask;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _initTask = InitializeUgs();
    }
    
    // Public Async Methods (For code scripts using 'await')

    public async Task InitializeServices()
    {
        if (_initTask == null || _initTask.IsFaulted)
        {
            _initTask = InitializeUgs();
        }
        await _initTask;
    }

    public async Task UpdatePlayerName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) return;

        PlayerPrefs.SetString(NameKey, newName);
        PlayerPrefs.Save();

        if (UnityServices.State != ServicesInitializationState.Initialized && _initTask != null)
        {
            await _initTask;
        }

        if (UnityServices.State != ServicesInitializationState.Initialized || !AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogError("[UGS] Services not ready to update name.");
            return;
        }

        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            Debug.Log($"[UGS] Player name updated: {newName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[UGS] Failed to update player name: {e}");
        }
    }

    public async Task SubmitScore(int score)
    {
        if (UnityServices.State != ServicesInitializationState.Initialized && _initTask != null)
        {
            await _initTask;
        }

        if (UnityServices.State != ServicesInitializationState.Initialized || !AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogError("[Leaderboard] Unity Services not initialized.");
            return;
        }

        try
        {
            var result = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            Debug.Log($"[Leaderboard] Score submitted successfully! Score: {result.Score}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Leaderboard] SCORE SUBMISSION FAILED:\n{e}");
        }
    }

    public async Task LoadLeaderboard()
    {
        Debug.Log("[Leaderboard] Loading leaderboard...");

        if (UnityServices.State != ServicesInitializationState.Initialized && _initTask != null)
        {
            await _initTask;
        }

        if (UnityServices.State != ServicesInitializationState.Initialized || !AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogError("[Leaderboard] Unity Services not ready or player not signed in.");
            return;
        }

        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId);

            foreach (Transform child in leaderboardContent)
            {
                Destroy(child.gameObject);
            }

            foreach (var entry in scores.Results)
            {
                PlayerScoreLeaderboardItem item = Instantiate(leaderboardItemPrefab, leaderboardContent);
                item.SetData(entry.Rank, entry.PlayerName, entry.Score);
            }

            Debug.Log("[Leaderboard] UI populated successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Leaderboard] Failed to load leaderboard:\n{e}");
        }
    }
    
    public async Task<int> GetPlayerHighScore()
    {
        await InitializeServices();

        if (UnityServices.State != ServicesInitializationState.Initialized || !AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogError("[Leaderboard] Unity Services failed to initialize or sign in.");
            return 0;
        }

        try
        {
            var playerEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardId);
            return (int)playerEntry.Score;
        }
        catch (Exception)
        {
            return 0;
        }
    }
    
    // Public Void Methods

    public void RefreshLeaderboardUI()
    {
        _ = LoadLeaderboard();
    }

    public void SubmitScoreVoid(int score)
    {
        _ = SubmitScore(score);
    }

    public void UpdatePlayerNameVoid(string newName)
    {
        _ = UpdatePlayerName(newName);
    }
    
    // ------------------------------------------------

    private async Task InitializeUgs()
    {
        try
        {
            Debug.Log("[UGS] Starting initialization...");

            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Debug.Log($"[UGS] Signed in. Player ID: {AuthenticationService.Instance.PlayerId}");

            string savedName = PlayerPrefs.GetString(NameKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(savedName))
            {
                try
                {
                    await AuthenticationService.Instance.UpdatePlayerNameAsync(savedName);
                    Debug.Log($"[UGS] Player name restored: {savedName}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[UGS] Failed to restore player name: {e}");
                }
            }

            await LoadLeaderboard();
            
            Debug.Log("[UGS] Initialization complete.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[UGS] Initialization FAILED: {e}");
        }
    }
}