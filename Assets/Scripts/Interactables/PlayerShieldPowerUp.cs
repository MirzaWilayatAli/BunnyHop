using UnityEngine;
using System.Collections;

public class PlayerShieldPowerUp : MonoBehaviour
{
    [SerializeField] private bool isShielded = false;
    public bool IsShielded => isShielded;

    public float shieldDuration = 6f;
    public float maxShieldDuration = 10f;
    public GameObject shieldVisual;
    public Color normalShieldColor = Color.white;
    public Color warningShieldColor = Color.red;
    public float flashInterval = 0.2f;

    private Material shieldMaterial;
    private Coroutine shieldRoutine;
    private Coroutine flashRoutine;
    private float currentShieldTimeRemaining;

    private void Awake()
    {
        if (shieldVisual != null)
        {
            Renderer renderer = shieldVisual.GetComponent<Renderer>();
            if (renderer != null)
            {
                shieldMaterial = renderer.material;
            }
        }
    }

    public void ActivateShield()
    {
        // Stop any existing routines
        if (shieldRoutine != null)
        {
            StopCoroutine(shieldRoutine);
        }
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        // Add duration (capped at max)
        currentShieldTimeRemaining = Mathf.Min(
            (isShielded ? currentShieldTimeRemaining : 0) + shieldDuration,
            maxShieldDuration
        );

        // Reset visual state
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
            if (shieldMaterial != null)
            {
                shieldMaterial.color = normalShieldColor;
            }
        }

        // Start/restart shield routine
        isShielded = true;
        shieldRoutine = StartCoroutine(ShieldRoutine());
    }

    private IEnumerator ShieldRoutine()
    {
        while (currentShieldTimeRemaining > 0)
        {
            float timeBeforeWarning = currentShieldTimeRemaining - 2f;

            if (timeBeforeWarning > 0)
            {
                // Normal phase - wait until warning or time runs out
                float waitTime = Mathf.Min(Time.deltaTime, timeBeforeWarning);
                yield return new WaitForSeconds(waitTime);
                currentShieldTimeRemaining -= waitTime;
            }
            else
            {
                // Warning phase - start flashing
                if (flashRoutine == null && shieldMaterial != null)
                {
                    flashRoutine = StartCoroutine(FlashRoutine());
                }

                float waitTime = Mathf.Min(Time.deltaTime, currentShieldTimeRemaining);
                yield return new WaitForSeconds(waitTime);
                currentShieldTimeRemaining -= waitTime;
            }
        }

        // Shield expired
        isShielded = false;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
            if (shieldMaterial != null)
            {
                shieldMaterial.color = normalShieldColor;
            }
        }
    }

    private IEnumerator FlashRoutine()
    {
        bool useWarningColor = false;
        while (isShielded && currentShieldTimeRemaining <= 2f)
        {
            useWarningColor = !useWarningColor;
            shieldMaterial.color = useWarningColor ? warningShieldColor : normalShieldColor;
            yield return new WaitForSeconds(flashInterval);
        }
        // Ensure we return to normal color when done
        if (shieldMaterial != null)
        {
            shieldMaterial.color = normalShieldColor;
        }
    }
}