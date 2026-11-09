using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var shield = other.gameObject.GetComponent<PlayerShieldPowerUp>();
            if (shield != null && shield.IsShielded)
            {
                return;
            }
            if (GameManager.Instance)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}
