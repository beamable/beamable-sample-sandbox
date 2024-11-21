using System.Collections.Generic;
using Beamable;
using Beamable.Server.Clients;
using UnityEngine;

public class StatExample : MonoBehaviour
{
    private BeamContext _context;
    private ServiceClient _service;

    private const string StatKey = "is_vip";
    private const string Access = "public";
    private const string Domain = "client";
    private const string Type = "player";

    private async void Start()
    {
        _context = await BeamContext.Default.Instance;
        _service = new ServiceClient();

        Debug.Log($"Player ID: {_context.PlayerId}");
        
        await _service.SetIsVipStat(_context.PlayerId);
        Debug.Log("Stat updated using service.");

        GetStats();

        var setStats = new Dictionary<string, string> { { StatKey, "false" } };
        await _context.Api.StatsService.SetStats(Access, setStats);
        Debug.Log("Stat updated using client.");
        
        GetStats();

    }

    private async void GetStats()
    {
        Dictionary<string, string> fetchedStats = await _context.Api.StatsService.GetStats(Domain, Access, Type, _context.PlayerId);

        if (fetchedStats.TryGetValue(StatKey, out string fetchedValue))
        {
            Debug.Log($"Fetched stat value: {fetchedValue}");
        }
        else
        {
            Debug.Log($"Stat '{StatKey}' not found.");
        }
    }
}