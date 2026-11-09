// Mirza Wilayat Ali (1034162)
// PowerUp.cs
using BunnyHop.Audio;
using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour
{
    public InteractableData data;
    [SerializeField] private int value = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            if (GameManager.Instance)
            {
                if (data != null && data.isPowerUp)
                {
                    ApplyEffect(data.powerUpEffect, other.gameObject);
                }

                // Increase score by the value
                GameManager.Instance.IncreaseScore(value);

                // Play Sound Effect
                SoundManager.Instance.PlayCollectableSound();

                // Instantly hide
                gameObject.SetActive(false);
            }
        } 
    }

    private void ApplyEffect(string effectType, GameObject player)
    {
        switch (effectType)
        {
            case "Shield":
                PlayerShieldPowerUp shield = player.GetComponent<PlayerShieldPowerUp>();
                if (shield != null)
                {
                    shield.ActivateShield();
                }
                break;
            case "ScoreBoost":
                GameManager.Instance.IncreaseScore(9);
                break;
            default:
                Debug.Log("Unknown power-up effect");
                break;
        }
    }
}
