using System.Threading.Tasks;
using Beamable;
using Beamable.Server.Clients;
using UnityEngine;

public class LobbiesExample : MonoBehaviour
{
    private BeamContext _beamContext;
    private ServiceClient _service;
    private long PlayerId { get; set; }
    private const string LobbyName = "MyTestLobby0";
    private const float HeartbeatInterval = 5f;

    private void Start()
    {
        SetupBeamable();
    }

    private async void SetupBeamable()
    {
        _beamContext = await BeamContext.Default.Instance;
        PlayerId = _beamContext.PlayerId;
        Debug.Log($"Beamable initialized with PlayerId: {PlayerId}");

        _service = new ServiceClient();

        // Check if the lobby already exists, and either create or join it
        await CheckOrCreateLobby();
    }

    // Method to check if the lobby exists, and create or join it accordingly
    private async Task CheckOrCreateLobby()
    {
        Debug.Log($"Checking for lobby: {LobbyName}");

        // Try to get the lobby by its name
        var getLobbyResponse = await _service.GetLobby(LobbyName);
        if (getLobbyResponse.errorMessage == "")
        {
            Debug.Log($"Current host of lobby {LobbyName}: {getLobbyResponse.data.hostId}");

            // Lobby exists, try to join
            Debug.Log($"Lobby {LobbyName} found, adding player {PlayerId} as a member.");
            await JoinLobby();
        }
        else
        {
            // Lobby doesn't exist, create a new one
            Debug.Log($"Lobby {LobbyName} not found, creating a new one.");
            await CreateLobby();
        }
    }

    // Method to create a new lobby
    private async Task CreateLobby()
    {
        var createLobbyResponse = await _service.CreateLobby(LobbyName, PlayerId);

        if (createLobbyResponse.errorMessage == "")
        {
            Debug.Log($"Lobby {LobbyName} successfully created with host: {PlayerId}");
        }
        else
        {
            Debug.LogError($"Error creating lobby: {createLobbyResponse.errorMessage}");
        }
    }

    // Method to join an existing lobby
    private async Task JoinLobby()
    {
        var addMemberResponse = await _service.AddMemberToLobby(LobbyName, PlayerId);

        if (addMemberResponse.errorMessage == "")
        {
            Debug.Log($"Player {PlayerId} successfully added to lobby {LobbyName}");
        }
        else
        {
            Debug.LogError($"Error joining lobby: {addMemberResponse.errorMessage}");
        }
    }
}
