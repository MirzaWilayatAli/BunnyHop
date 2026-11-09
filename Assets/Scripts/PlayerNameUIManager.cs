using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class PlayerNameUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject enterNamePanel;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_Text nameDisplayText;
    [SerializeField] private TMP_Text highScoreDisplayText; // New text reference for score

    private const string NameKey = "PlayerName";

    private async void Start()
    {
        await RefreshUIAsync();
    }

    public async Task RefreshUIAsync()
    {
        string savedName = PlayerPrefs.GetString(NameKey, string.Empty);
        bool hasName = !string.IsNullOrWhiteSpace(savedName);

        if (nameDisplayText != null)
        {
            nameDisplayText.text = savedName;
        }

        if (enterNamePanel != null)
        {
            enterNamePanel.SetActive(!hasName);
        }

        // Fetch high score if player has a name set
        if (hasName && LeaderboardManager.Instance != null && highScoreDisplayText != null)
        {
            highScoreDisplayText.text = "High Score: Loading...";
            
            // Call your LeaderboardManager score fetch method
            int highScore = await LeaderboardManager.Instance.GetPlayerHighScore();
            
            highScoreDisplayText.text = $"High Score: {highScore}";
        }
        else if (highScoreDisplayText != null)
        {
            highScoreDisplayText.text = "High Score: 0";
        }
    }

    public async void ConfirmAndSaveName()
    {
        if (nameInputField == null || string.IsNullOrWhiteSpace(nameInputField.text)) 
            return;

        string enteredName = nameInputField.text.Trim();

        if (LeaderboardManager.Instance != null)
        {
            await LeaderboardManager.Instance.UpdatePlayerName(enteredName);
        }

        await RefreshUIAsync();
    }
}