using System.Collections;
using System.Collections.Generic;
using Beamable;
using UnityEngine;
using Beamable.Server.Clients;

public class StatExample : MonoBehaviour
{
    void Start()
    {
        if (ShouldTrackMetrics())
        {
            // Your tracking logic here
            TrackInstall();
            TrackDAU();
        }
        else
        {
            Debug.Log("Metrics tracking skipped for this platform.");
        }
    }

    private bool ShouldTrackMetrics()
    {
        // Exclude specific platforms
        return !(Application.platform == RuntimePlatform.WindowsEditor ||
                 Application.platform == RuntimePlatform.LinuxServer ||
                 Application.platform == RuntimePlatform.OSXEditor);
    }

    private void TrackInstall()
    {
        // Logic to track install
        Debug.Log("Install tracked.");
    }

    private void TrackDAU()
    {
        // Logic to track DAU
        Debug.Log("DAU tracked.");
    }
}