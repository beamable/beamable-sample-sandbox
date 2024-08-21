using System.Collections;
using System.Collections.Generic;
using Beamable;
using UnityEngine;
using Beamable.Server.Clients;

public class StatExample : MonoBehaviour
{
    private ServiceClient _service;

    //  Unity Methods  --------------------------------
    protected void Start()
    {
        _service = new ServiceClient();

        SetupBeamable();
    }

    private async void SetupBeamable()
    {
        var context = BeamContext.Default;
        await context.OnReady;

        Debug.Log($"context.PlayerId = {context.PlayerId}");

        string statKey = "MyExampleStat";
        string access = "public";
        string domain = "client";
        string type = "player";
        long id = context.PlayerId;

        await _service.ResetStats(statKey);

        // Get Value
        Dictionary<string, string> getStats =
            await context.Api.StatsService.GetStats(domain, access, type, id);

        string myExampleStatValue = "";
        getStats.TryGetValue(statKey, out myExampleStatValue);

        Debug.Log($"myExampleStatValue = {myExampleStatValue}");
    }
}