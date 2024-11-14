using System.Collections.Generic;
using Beamable;
using Beamable.Server.Clients;
using UnityEngine;

public class DeleteAccounts : MonoBehaviour
{
    private BeamContext _beamContext;

    async void Start()
    {
        _beamContext = await BeamContext.Default.Instance;

        Debug.Log($"Current PlayerId: {_beamContext.PlayerId}");
        Debug.Log($"Token: {_beamContext.AccessToken.Token}");

        // Get all player IDs from the microservice
        var client = new ServiceClient();
        var playerIds = await client.ListAllPlayers();

        Debug.Log("Players to delete: " + string.Join(", ", playerIds));

        // Delete each account
        foreach (var playerId in playerIds)
        {
            Debug.Log(playerId);
            try
            {
                var response = await client.DeleteAccount(playerId);
                Debug.Log($"Deleted PlayerId: {playerId}, Response: {response}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to delete PlayerId: {playerId}, Error: {ex.Message}");
            }
        }

        await Beam.ClearAndStopAllContexts();
    }
}