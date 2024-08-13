using System;
using UnityEngine;
using Beamable.Common.Api.Groups;
using Beamable.Server.Clients;
using System.Threading.Tasks;
using Beamable;

public class CreateGroups : MonoBehaviour
{
    private BeamContext _beamContext;

    async void Start()
    {
        _beamContext = await BeamContext.Default.Instance;
        
        string groupName = "Test Group";

        bool isNameAvailable = await CheckGroupNameAvailability(groupName);
        
        if (isNameAvailable)
        {
            string result = await CreateGroupWithSpaces(groupName);
            Debug.Log(result);
        }
        else
        {
            Debug.Log($"Group name '{groupName}' is unavailable.");
        }
    }

    private async Task<bool> CheckGroupNameAvailability(string groupName)
    {
        try
        {
            var checkAvailabilityResponse = await _beamContext.Api.GroupsService.CheckAvailability(groupName, "000");
            return checkAvailabilityResponse.name;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error checking availability: {e.Message}");
            return false;
        }
    }

    private async Task<string> CreateGroupWithSpaces(string groupName)
    {
        try
        {
            var service = new ServiceClient();
            var result = await service.CreateGroupWithSpaces(groupName);
            return result;
        }
        catch (Exception e)
        {
            return $"ERROR: {e.Message}";
        }
    }
}