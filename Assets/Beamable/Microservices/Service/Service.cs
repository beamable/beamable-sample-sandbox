using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Models;
using Beamable.Common.Utils;
using Beamable.Mongo;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public async Promise<Response<LobbyData>> ChangeHost(string lobbyName)
        {
            try
            {
                // Retrieve the lobby by its name
                var lobbyData = await Storage.GetByFieldName<LobbyData, string>("lobbyName", lobbyName);
                if (lobbyData == null || lobbyData.memberIds.Count == 0)
                {
                    Debug.LogError("Lobby not found or no members available.");
                    return new Response<LobbyData>(null, "Lobby not found or no members available.");
                }

                // Filter out the current host from potential new hosts
                var potentialNewHosts = lobbyData.memberIds.FindAll(memberId => memberId != lobbyData.hostId);

                if (potentialNewHosts.Count == 0)
                {
                    Debug.LogWarning("No other members available to assign as host.");
                    return new Response<LobbyData>(null, "No other members available to assign as host.");
                }

                // Select the first available member as the new host
                var newHostId = potentialNewHosts[0]; // Could add additional logic for selecting the new host

                // Update the lobby with the new host
                lobbyData.hostId = newHostId;

                // Save the updated lobby data
                await Storage.Update(lobbyData.Id, lobbyData);
                Debug.Log($"New host for lobby {lobbyName} is {newHostId}");

                // Return the updated lobby data
                return new Response<LobbyData>(lobbyData);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return new Response<LobbyData>(null, "Error changing host");
            }
        }

        
        // Method to create a new lobby with a unique name
        [ClientCallable]
        public async Promise<Response<LobbyData>> CreateLobby(string lobbyName, long hostId)
        {
            Debug.Log("In create lobby");

            // Check if a lobby with the same name already exists
            var existingLobby = await Storage.GetByFieldName<LobbyData, string>("lobbyName", lobbyName);
            if (existingLobby != null)
            {
                Debug.Log($"Lobby with name {lobbyName} already exists.");
                return new Response<LobbyData>(null, "Lobby with this name already exists");
            }

            // Create a new lobby object
            var lobbyData = new LobbyData
            {
                lobbyName = lobbyName,
                hostId = hostId,
                memberIds = new List<long> { hostId } // Add host as the first member
            };
            Debug.Log("Created lobby object");

            try
            {
                await Storage.Create<ServiceDataStorage, LobbyData>(lobbyData);
                return new Response<LobbyData>(lobbyData);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return new Response<LobbyData>(null, "Error creating lobby");
            }
        }

        // Method to get a lobby by lobby name
        [ClientCallable]
        public async Promise<Response<LobbyData>> GetLobby(string lobbyName)
        {
            try
            {
                var lobbyData = await Storage.GetByFieldName<LobbyData, string>("lobbyName", lobbyName);
                return lobbyData == null ? new Response<LobbyData>(null, "Lobby not found") : new Response<LobbyData>(lobbyData);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return new Response<LobbyData>(null, "Error retrieving lobby");
            }
        }

        // Method to add a member to the lobby by lobby name, ensuring no duplicates
        [ClientCallable]
        public async Promise<Response<bool>> AddMemberToLobby(string lobbyName, long memberId)
        {
            try
            {
                // Retrieve the lobby by its name
                var lobbyData = await Storage.GetByFieldName<LobbyData, string>("lobbyName", lobbyName);
                if (lobbyData == null)
                    return new Response<bool>(false, "Lobby not found");

                // Check if the member is already in the lobby
                if (!lobbyData.memberIds.Contains(memberId))
                {
                    // Add the new member and update the lobby
                    lobbyData.memberIds.Add(memberId);
                    await Storage.Update(lobbyData.Id, lobbyData);
                    Debug.Log($"Added member {memberId} to lobby {lobbyName}");
                }
                else
                {
                    Debug.Log($"Member {memberId} is already in the lobby {lobbyName}");
                    return new Response<bool>(false, $"Member {memberId} is already in the lobby {lobbyName}");
                }

                return new Response<bool>(true);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return new Response<bool>(false, "Error adding member to lobby");
            }
        }

        // Method to set a new host by lobby name
        [ClientCallable]
        public async Promise<Response<bool>> SetHost(string lobbyName, long newHostId)
        {
            try
            {
                // Retrieve the lobby by its name
                var lobbyData = await Storage.GetByFieldName<LobbyData, string>("lobbyName", lobbyName);
                if (lobbyData == null)
                    return new Response<bool>(false, "Lobby not found");

                // Set the new host and update the lobby
                lobbyData.hostId = newHostId;
                await Storage.Update(lobbyData.Id, lobbyData);
                Debug.Log($"Host of lobby {lobbyName} set to {newHostId}");

                return new Response<bool>(true);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return new Response<bool>(false, "Error setting host");
            }
        }
    }
}
