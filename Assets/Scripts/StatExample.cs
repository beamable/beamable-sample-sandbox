using System.Collections;
using System.Collections.Generic;
using Beamable;
using UnityEngine;
using Beamable.Server.Clients;

public class StatExample : MonoBehaviour
{
    private ServiceClient _service;

    // Unity Methods  --------------------------------
    void Start()
    {
        // Your tracking logic here
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            TrackInstall();
            TrackDAU();
        }
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
