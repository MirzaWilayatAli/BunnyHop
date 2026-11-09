// PHOEBE (1033478)
// SCREENMANAGER.CS

using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance;
    private void Awake()
    {
        // Singleton stuff, we don't really want more than one of these existing
        // at any time. That'd be bad.
        if (Instance == null)
        {
            Logger.Log(this, $"Singleton set.");
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Logger.Log(this, $"Singleton already set.");
            Destroy(gameObject);
        }

        // defaults are LandscapeLeft + refreshRate * 2
        SetMaxFramerate((int)Screen.currentResolution.refreshRateRatio.value);
        ChangeRotation(ScreenOrientation.LandscapeLeft); 
    }

    public void ChangeRotation(ScreenOrientation orientation)
    {
        // Cleaner wrapper for picking orientation + logging
        Logger.Log(this, $"Changing rotation to {orientation}.");
        Screen.orientation = orientation;
    }
    
    public ScreenOrientation GetScreenOrientation() => Screen.orientation;

    public void SetMaxFramerate(int maxFramerate, bool doubled = true)
    {
        // Clean wrapper for setting maximum framerate
        
        // Doubling it makes it appear smoother, but at a slightly higher performance cost. It should be up to the player to enable it, but for the sake of testing: it's on by default.
        if (doubled)
        {
            Application.targetFrameRate = maxFramerate * 2; 
            Logger.Log(this, $"Changing max framerate to {maxFramerate * 2}.");
        }
        else
        {
            Application.targetFrameRate = maxFramerate;
            Logger.Log(this, $"Changing max framerate to {maxFramerate}.");
        }
    }
}
