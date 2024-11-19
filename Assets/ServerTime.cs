using System.Collections;
using System.Collections.Generic;
using Beamable;
using Beamable.Api;
using Beamable.Common.Api;
using UnityEngine;

public class ServerTime : MonoBehaviour
{
    public long latestServerTimestamp;

    private BeamContext ctx;
    private IPlatformRequester _observer;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Initializing BeamContext and IPlatformRequester...");
        ctx = BeamContext.Default;

        if (ctx == null)
        {
            Debug.LogError("BeamContext is not initialized. Ensure Beamable is set up correctly.");
            return;
        }

        _observer = ctx.ServiceProvider.GetService<IPlatformRequester>();

        if (_observer == null)
        {
            Debug.LogError("IPlatformRequester could not be retrieved. Check your Beamable setup.");
        }
        else
        {
            Debug.Log("IPlatformRequester successfully initialized.");
        }
    }

    void Update()
    {

            latestServerTimestamp = _observer.GetLatestServerTimestamp();

    }
}