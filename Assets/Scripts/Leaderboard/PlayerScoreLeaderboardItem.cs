using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreLeaderboardItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image backgroundImage;

    [Header("Rank Colors")]
    [SerializeField] private Color goldColor = new Color(1f, 0.84f, 0f);      // #FFD700
    [SerializeField] private Color silverColor = new Color(0.75f, 0.75f, 0.75f); // #C0C0C0
    [SerializeField] private Color bronzeColor = new Color(0.80f, 0.50f, 0.20f); // #CD7F32
    [SerializeField] private Color defaultColor;

    public void SetData(int rank, string playerName, double score)
    {
        rankText.text = (rank + 1).ToString();

        if (!string.IsNullOrEmpty(playerName) && playerName.Contains("#"))
        {
            playerName = playerName.Split('#')[0];
        }

        playerNameText.text = playerName;
        scoreText.text = score.ToString("0");

        // Apply colors based on 0-indexed rank
        Color itemColor = rank switch
        {
            0 => goldColor,
            1 => silverColor,
            2 => bronzeColor,
            _ => defaultColor
        };

        if (backgroundImage != null) backgroundImage.color = itemColor;
    }
}