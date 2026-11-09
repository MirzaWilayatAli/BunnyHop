using BunnyHop.Audio;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider bgmSlider;  // Assign in Inspector
    [SerializeField] private Slider sfxSlider;  // Assign in Inspector

    private void Start()
    {
        // If SoundManager exists, initialize sliders with current volumes
        if (SoundManager.Instance != null)
        {
            // Set slider values to current volumes
            bgmSlider.value = SoundManager.Instance.GetBGVolume();
            sfxSlider.value = SoundManager.Instance.GetSFXVolume();

            // Add listeners to update volume when sliders change
            bgmSlider.onValueChanged.AddListener(UpdateBGMVolume);
            sfxSlider.onValueChanged.AddListener(UpdateSFXVolume);
        }
        else
        {
            Debug.LogWarning("SoundManager not found! Is it in the scene?");
        }
    }

    private void UpdateBGMVolume(float volume)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ChangeBGVolume(volume);
        }
    }

    private void UpdateSFXVolume(float volume)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ChangeSFXVolume(volume);
        }
    }
}