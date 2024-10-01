using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable;
using Beamable.Experimental.Api.Lobbies;
using UnityEngine;

namespace DefaultNamespace
{
    public class LobbiesExample : MonoBehaviour
    {
        private BeamContext _beamContext;
        private Lobby ActiveLobby { get; set; }
        private string PlayerId { get; set; }
        private const string LobbyName = "MyTestLobby";

        private void Start()
        {
            SetupBeamable();
        }

        private async void SetupBeamable()
        {
            _beamContext = await BeamContext.Default.Instance;
            _beamContext.Lobby.OnDataUpdated += OnLobbyDataUpdated;
            PlayerId = _beamContext.PlayerId.ToString();
            Debug.Log($"Beamable initialized with PlayerId: {PlayerId}");

            // Check if the lobby already exists, and either create or join it
            await HandleLobbyCreationOrJoin();
        }

        private void OnDestroy()
        {
            if (_beamContext != null)
            {
                _beamContext.Lobby.OnDataUpdated -= OnLobbyDataUpdated;
            }
        }

        private async Task HandleLobbyCreationOrJoin()
        {
            // Search for existing lobbies
            var existingLobby = await FindExistingLobby(LobbyName);

            if (existingLobby != null)
            {
                Debug.Log($"Lobby '{LobbyName}' already exists. Joining...");
                await JoinExistingLobby(existingLobby);
            }
            else
            {
                // If no existing lobby, create a new one
                await CreateLobby();
            }
        }

        private async Task<Lobby> FindExistingLobby(string lobbyName)
        {
            try
            {
                Debug.Log("Searching for existing lobbies...");
                var lobbies = (await _beamContext.Lobby.FindLobbies()).results;

                foreach (var lobby in lobbies.Where(lobby => lobby.name == lobbyName))
                {
                    Debug.Log($"Found existing lobby: {lobby.name}");
                    return lobby;
                }
                Debug.Log("No existing lobby found.");
                return null;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error finding existing lobby: {ex.Message}");
                return null;
            }
        }

        private async Task JoinExistingLobby(Lobby lobby)
        {
            try
            {
                // Check if the player is already in the lobby
                if (lobby.players.Exists(player => player.playerId == PlayerId))
                {
                    Debug.Log($"Player {PlayerId} is already in the lobby: {lobby.name}. No need to join again.");
                    return; // Player is already in the lobby; do not attempt to join
                }

                // Join the existing lobby using the lobbyId directly
                await _beamContext.Lobby.Join(lobby.lobbyId);
                Debug.Log($"Successfully joined the lobby: {lobby.name}");

                // Add second player to the lobby
                await AddPlayerToLobby();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error joining lobby: {ex.Message}");
            }
        }


        private async Task CreateLobby()
        {
            var lobbyRecord = new CreateLobbyRecord
            {
                Name = LobbyName,
                Restriction = LobbyRestriction.Open, // Open so others can join
                GameTypeId = "game_types.defaultGameType", // Replace with your game type ID
                Description = "Test Lobby for 2 players",
                MaxPlayers = 2 // Setting max players to 2
            };

            try
            {
                Debug.Log("Creating new lobby...");
                await _beamContext.Lobby.Create(
                    lobbyRecord.Name,
                    lobbyRecord.Restriction,
                    lobbyRecord.GameTypeId,
                    lobbyRecord.Description,
                    lobbyRecord.PlayerTags,
                    lobbyRecord.MaxPlayers,
                    lobbyRecord.PasscodeLength
                );
                Debug.Log("Lobby created successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error creating lobby: {ex.Message}");
            }
        }

        private async Task AddPlayerToLobby()
        {
            try
            {
                var lobby = ActiveLobby;
                if (lobby != null)
                {
                    // Join the lobby with the lobbyId and playerTags
                    await _beamContext.Lobby.Join(lobby.lobbyId);
                }
                else
                {
                    Debug.LogError("Cannot add player. No active lobby found.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error adding player to lobby: {ex.Message}");
            }
        }


        private void OnLobbyDataUpdated(Lobby lobby)
        {
            if (lobby == null) return;
            ActiveLobby = lobby;

            Debug.Log($"Lobby updated: {ActiveLobby.name}, Host: {ActiveLobby.host}, Players: {ActiveLobby.players.Count}/{ActiveLobby.maxPlayers}");

            // Log details of each player in the lobby
            foreach (var player in ActiveLobby.players)
            {
                Debug.Log($"Player ID: {player.playerId}");
            }
        }
        
        private async Task AssignNewHostAndLeaveLobby()
        {
            if (ActiveLobby == null)
            {
                Debug.LogError("No active lobby found.");
                return;
            }

            if (ActiveLobby.host != PlayerId)
            {
                Debug.LogError("Only the current host can assign a new host.");
                return;
            }

            // Find another player to assign as the new host (excluding the current host)
            var newHost = ActiveLobby.players.Find(player => player.playerId != PlayerId);

            if (newHost == null)
            {
                Debug.LogError("No other players available to assign as the new host.");
                return;
            }

            try
            {
                // Update the lobby to assign the new host
                await _beamContext.Lobby.Update(
                    ActiveLobby.lobbyId, // lobby ID
                    LobbyRestriction.Open, // keep the same restriction (e.g., Open or Closed)
                    newHost.playerId, // new host player ID
                    ActiveLobby.name, // keep the same lobby name
                    ActiveLobby.description, // keep the same description
                    "game_types.defaultGameType", // keep the same game type ID
                    ActiveLobby.maxPlayers // keep the same max players
                );

                Debug.Log($"New host {newHost.playerId} has been assigned. Leaving lobby now...");

                // After assigning the new host, the current host can leave
                Debug.Log($"New Host: {_beamContext.Lobby.Host}");
                await _beamContext.Lobby.Leave();
                Debug.Log("Current host has left the lobby.");

            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error assigning new host or leaving the lobby: {ex.Message}");
            }
        }

        public async void RemoveHostButton()
        {
            await AssignNewHostAndLeaveLobby();
        }
    }
    


    public record CreateLobbyRecord
    {
        public string Name { get; set; }
        public LobbyRestriction Restriction { get; set; }
        public string GameTypeId { get; set; }
        public string Description { get; set; }
        public List<Tag> PlayerTags { get; set; }
        public int? MaxPlayers { get; set; }
        public int? PasscodeLength { get; set; }
    }
    
}
