using Beamable.Common;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Beamable;
using Beamable.Experimental.Api.Parties;
using System.Collections.Generic;
using System.Linq;
using Beamable.Player;

public class BeamableParty : MonoBehaviour
{
    private BeamContext _beamContext;

    [SerializeField] private string PlayerId;

    private async void Start()
    {
        // Initialize BeamContext
        _beamContext = await BeamContext.Default.Instance;
        Debug.Log($"PlayerId: {_beamContext.PlayerId}");

        // Subscribe to party invites
        SubscribeToPartyInvites();

        // Check and create a party if not already in one
        await EnsurePartyExists();
    }

    private async Task EnsurePartyExists()
    {
        if (!_beamContext.Party.IsInParty)
        {
            try
            {
                // Create a new party with no restrictions
                await _beamContext.Party.Create(PartyRestriction.Unrestricted);
                Debug.Log("Party created successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create party: {e.Message}");
            }
        }
        else
        {
            Debug.Log("Party exists");
        }
    }

    public async Task<InviteToPartyResult> InvitePlayerToParty()
    {
        InviteToPartyResult invitationResult = new InviteToPartyResult();

        await EnsurePartyExists();

        if (!_beamContext.Party.IsInParty || !_beamContext.Party.IsLeader)
        {
            invitationResult.error = "Not able to invite. You must be in a party and be the party leader.";
            return invitationResult;
        }

        try
        {
            await _beamContext.Party.Invite(PlayerId);
            Debug.Log($"Invitation sent to Player ID: {PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inviting player {PlayerId}: {e.Message}");
            invitationResult.error = e.Message;
        }

        return invitationResult;
    }


    public async void InviteButton()
    {
        var result = await InvitePlayerToParty();
        Debug.Log("Completed: " + (string.IsNullOrEmpty(result.error) ? "Success" : $"Error: {result.error}") + $" To: {PlayerId}");
    }

    private void SubscribeToPartyInvites()
    {
        // Subscribe to the received party invites
        _beamContext.Party.ReceivedPartyInvites.OnElementsAdded += invites =>
        {
            var receivedPartyInvites = invites as ReceivedPartyInvite[] ?? invites.ToArray();
            Debug.Log($"Received {receivedPartyInvites.Count()} new party invite(s):");

            foreach (var invite in receivedPartyInvites)
            {
                Debug.Log($"Invite from Player ID: {invite.invitedBy}, Party ID: {invite.partyId}");
                // You can automatically accept an invite if needed
                // invite.Accept();
            }
        };

        // Fetch existing invites on start
        ListExistingInvites();
    }

    private async void ListExistingInvites()
    {
        try
        {
            var invitesResponse = await _beamContext.Party.GetInvites();

            if (invitesResponse.invitations.Count > 0)
            {
                Debug.Log($"You have {invitesResponse.invitations.Count()} pending party invite(s):");
                foreach (var invite in invitesResponse.invitations)
                {
                    Debug.Log($"Invite from Player ID: {invite.invitedBy}, Party ID: {invite.partyId}");
                }
            }
            else
            {
                Debug.Log("No pending party invites.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching party invites: {e.Message}");
        }
    }
}

public class InviteToPartyResult
{
    public string error;
}
