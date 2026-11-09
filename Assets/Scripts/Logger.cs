// PHOEBE FAITH (1033478)
// LOGGER.CS

// This isn't strictly necessary, but it makes disabling logging for higher performance testing easier :)

using UnityEngine;

public static class Logger
{
        // If you are missing a GameManager Instance, it will log but with (noGM) appended to the sender type. Realistically, if you've got no GameManager, something's fucked up lol
        // Logger.Log(this, "message")
        //
        // You should ideally use this for logging over raw Debug.Log because
        // this can be disabled at any point. Makes it easier to start/stop
        // debug logging so you don't clog the console at all times!!
        public static void Log(object sender, string message)
        {
                if(GameManager.Instance) 
                {
                   if(GameManager.Instance.shouldLog) Debug.Log($"{sender}: {message}");
                } else {
                    Debug.Log($"{sender} (noGM): {message}");
                }
        }
}
