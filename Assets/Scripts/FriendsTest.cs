using System.Collections;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Presence;
using Beamable.Player;
using UnityEngine;

public class FriendsTest : MonoBehaviour
{
    private long playerToBefriend = 1788697549374465; // Player ID for sending invite
    public PlayerSocial social;
    private BeamContext _ctx;

    // Start is called before the first frame update
    async void Start()
    {
        _ctx = await BeamContext.Default.Instance;
        Debug.Log($"player id: {_ctx.PlayerId}");
        social = _ctx.Social;

        // Listening for friend presence changes
        _ctx.Social.FriendPresenceChanged += friend =>
        {
            Debug.Log($"{_ctx.PlayerId} saw friend {friend.playerId} go online=[{friend.Presence.status}] - desc=[{friend.Presence.description}]");
        };

        // Listen for any updates to the friends list or other social data
        _ctx.Social.OnUpdated += () =>
        {
            Debug.Log("Social data updated.");
        };

        // Set custom status
        await SetCustomStatus(PresenceStatus.Away, "away from the computer");
    }

    // Method to send a friend invite
    public async void SendInvite()
    {
        await _ctx.Social.Invite(playerToBefriend);
        Debug.Log($"Invite sent to player {playerToBefriend}.");
    }

    // Accept a friend invite
    public async void AcceptInvite()
    {
        if (_ctx.Social.ReceivedInvites.Count > 0)
        {
            await _ctx.Social.ReceivedInvites[0].AcceptInvite();
            Debug.Log("Friend invite accepted.");
        }
        else
        {
            Debug.Log("No received invites to accept.");
        }
    }

    // Remove a friend
    public async void RemoveFriend()
    {
        if (_ctx.Social.Friends.Count > 0)
        {
            await _ctx.Social.Friends[0].Unfriend();
            Debug.Log("Friend removed.");
        }
        else
        {
            Debug.Log("No friends to remove.");
        }
    }

    // Cancel a sent invite
    public async void CancelSentInvite()
    {
        if (_ctx.Social.SentInvites.Count > 0)
        {
            await _ctx.Social.SentInvites[0].Cancel();
            Debug.Log("Sent invite canceled.");
        }
        else
        {
            Debug.Log("No sent invites to cancel.");
        }
    }

    // Method to set the player's custom presence status
    // Set custom status with description
    public async Task SetCustomStatus(PresenceStatus status, string description)
    {
        await _ctx.Presence.SetPlayerStatus(status, description);
        Debug.Log($"Set custom status: {status}, description: {description}");
    }
}
