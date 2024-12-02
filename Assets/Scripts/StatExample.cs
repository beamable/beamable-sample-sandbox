using Beamable;
using Beamable.Server.Clients;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class StatExample : MonoBehaviour
{
    private BeamEditorContext _adminContext;
    private BeamContext _beamContext;
    private ServiceClient _service;

    private const string StatKey = "is_vip";
    private const string Access = "public";
    private const string Domain = "client";
    private const string Type = "player";

    private async void Start()
    {
        // // Use BeamEditorContext for admin access
        // _adminContext = BeamEditorContext.Default;
        _beamContext = await BeamContext.Default.Instance;
        // _service = new ServiceClient();
        //
        Debug.Log($"Player id: {_beamContext.PlayerId}");
        // Debug.Log($"Admin Context Player ID: {_adminContext.CurrentUser.id}");
        //
        // // Get initial stats
        // GetStats();
        //
        // // Call the microservice as an admin
        // await _service.SetIsVipStat(_adminContext.CurrentUser.id);
        // Debug.Log("Stat updated using admin service.");

        // Get updated stats
        GetStats();
    }

    private async void GetStats()
    {
        Dictionary<string, string> fetchedStats = await _beamContext.Api.StatsService.GetStats(Domain, Access, Type, _beamContext.PlayerId);

        if (fetchedStats.TryGetValue(StatKey, out string fetchedValue))
        {
            Debug.Log($"Fetched stat value: {fetchedValue}");
        }
        else
        {
            Debug.Log($"Stat '{StatKey}' not found.");
        }
    }
    
    [MenuItem("Beamable/Admin/Set VIP Stat")]
    public static async void SetVipStat()
    {
        var editorContext = BeamEditorContext.Default;
        var service = new ServiceClient();


        await service.SetIsVipStat(1811038189543425);
        Debug.Log($"Stat updated for Player ID: {1811038189543425}");
    }
}