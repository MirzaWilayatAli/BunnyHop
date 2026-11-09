// PHOEBE FAITH (1033478)
// COLLECTABLE.CS

using BunnyHop.Audio;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private int value = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if player hit, and if valid GameManager instance
        if (other.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance)
            {
                // Increase score by the value
                GameManager.Instance.IncreaseScore(value);
                // Instantly hide
                gameObject.SetActive(false);

                // Play Sound Effect
                SoundManager.Instance.PlayCollectableSound();
            }
        }
    }
}
