using TMPro;
using UnityEngine;

public class ScoreSetter : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    public void UpdateScoreText()
    {
        // Fetch the current score from GameManager
        int score = GameManager.Instance.GetScore();
        scoreText.SetText($"{score}");
    }
}