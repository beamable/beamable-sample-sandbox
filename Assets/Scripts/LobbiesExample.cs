using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Presence;
using Beamable.Player;
using Beamable.Server.Clients;
using UnityEngine;

public class LobbiesExample : MonoBehaviour
{
    public PlayerSocial social;
    public long playerToBefriend;
    private long _currentHostId;
    private BeamContext _beamContext;
    private ServiceClient _service;
    private long PlayerId { get; set; }
    private const string LobbyName = "MyTestLobby2";

    private void Start()
    {
        SetupBeamable();
    }

    private async void SetupBeamable()
    {
        _beamContext = await BeamContext.Default.Instance;
        PlayerId = _beamContext.PlayerId;
        Debug.Log($"Beamable initialized with PlayerId: {PlayerId}");

        social = _beamContext.Social;
        _service = new ServiceClient();

        // Check if the lobby already exists, and either create or join it
        await CheckOrCreateLobby();

        social.FriendPresenceChanged += async friend =>
        {
            Debug.Log($"Friend {friend.playerId} online status: {friend.Presence.online}");

            // Only proceed if the current host went offline
            if (friend.playerId != _currentHostId || friend.Presence.online) return;
            Debug.Log("Host went offline. Reassigning host...");
            var changeHostResponse = await _service.ChangeHost(LobbyName);

            if (changeHostResponse.errorMessage == "")
            {
                Debug.Log($"New host for lobby {LobbyName}: {changeHostResponse.data.hostId}");
                _currentHostId = changeHostResponse.data.hostId;
            }
            else
            {
                Debug.LogError($"Error reassigning host: {changeHostResponse.errorMessage}");
            }
        };
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
            _currentHostId = getLobbyResponse.data.hostId;
            
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
    
    // Method to send a friend invite
    public async void SendInvite()
    {
        await social.Invite(playerToBefriend);
        Debug.Log($"Invite sent to player {playerToBefriend}.");
    }

    // Accept a friend invite
    public async void AcceptInvite()
    {
        if (social.ReceivedInvites.Count > 0)
        {
            await social.ReceivedInvites[0].AcceptInvite();
            Debug.Log("Friend invite accepted.");
        }
        else
        {
            Debug.Log("No received invites to accept.");
        }
    }
}
