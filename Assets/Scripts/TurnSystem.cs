using System;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Models;
using Beamable.Serialization.SmallerJSON;
using UnityEngine;
using Beamable.Server.Clients;
using TMPro;

public class TurnSystem : MonoBehaviour
{
    private ServiceClient _service; // Client for calling the microservice
    private BeamContext _beamContext; // Beamable context for notification service
    private bool isSubscribed;

    private long _playerId;
    [SerializeField]
    private long targetPlayerId = 1800376155037697; //the default ID is only for example purpose

    [SerializeField] private TMP_Text turnStatusText;

    // Subscribe to notifications and initialize the ServiceClient
    private async void Start()
    {
        try
        {
            // Initialize ServiceClient and BeamContext
            _service = new ServiceClient();
            _beamContext = await BeamContext.Default.Instance;
            _playerId = _beamContext.PlayerId;
            
            // Log the player's ID from the BeamContext
            Debug.Log($"PlayerId: {_playerId}");

            await CheckAndSetTargetPlayer();
            
            // Subscribe to Ping notifications
            _beamContext.Api.NotificationService.Subscribe("PingNotification", HandlePingNotification);
            isSubscribed = true;

            Debug.Log("Successfully subscribed to PingNotification.");
            
            await UpdateTurnStatus();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error initializing TurnSystem: {ex.Message}");
        }
    }

    // Handle ping notification when received
    private async void HandlePingNotification(object payload)
    {
        try
        {
            Debug.Log($"Received a PingNotification {payload}.");

            // Parse the payload to extract senderId
            if (payload is ArrayDict arrayDict)
            {
                if (arrayDict.TryGetValue("FromPlayer", out var senderIdObj))
                {
                    var senderId = Convert.ToInt64(senderIdObj);
                    Debug.Log($"Ping received from player: {senderId}");

                    // Initialize player and target player
                    InitializePlayers(_beamContext.PlayerId, senderId);
                    Debug.Log($"You've been pinged by {senderId}! Ready to ping back.");
                    await UpdateTurnStatus();
                }
                else
                {
                    Debug.LogError("Failed to extract senderId from payload.");
                }
            }
            else
            {
                Debug.LogError("Payload is not of type ArrayDict.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error handling PingNotification: {ex.Message}");
        }
    }

    // Function to send a ping to the target player
    private async Task SendPing()
    {
        try
        {
            Debug.Log($"Attempting to send ping from {_playerId} to {targetPlayerId}.");

            if (_playerId == 0 || targetPlayerId == 0)
            {
                Debug.LogError("Player ID or Target Player ID is not set. Cannot send ping.");
                return;
            }

            // Call the service to send a ping
            var response = await _service.Ping(_playerId, targetPlayerId);

            if (string.IsNullOrEmpty(response.errorMessage))
            {
                Debug.Log("Ping sent successfully.");
                await UpdateTurnStatus();
            }
            else
            {
                Debug.LogError($"Ping failed: {response.errorMessage}");
            }
            
            

        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending ping: {ex.Message}");
        }
    }

    // Call this to set player and target IDs
    private void InitializePlayers(long playerId, long targetPlayer)
    {
        Debug.Log($"Initializing players: PlayerId = {playerId}, TargetPlayerId = {targetPlayerId}");

        _playerId = playerId;
        targetPlayerId = targetPlayer;
    }

    // Unsubscribe when the object is destroyed
    private void OnDestroy()
    {
        if (!isSubscribed || _beamContext == null) return;

        Debug.Log("Unsubscribing from PingNotification...");
        _beamContext.Api.NotificationService.Unsubscribe("PingNotification", HandlePingNotification);
        isSubscribed = false;
        Debug.Log("Unsubscribed from PingNotification.");
    }

    // This method is triggered by a button press to send the ping
    public async void SendPingButton()
    {
        await SendPing();
    }
    
    // Function to check whose turn it is and update UI
    private async Task UpdateTurnStatus()
    {
        // Call the service to check the turn
        var response = await _service.IsPlayerTurn(_playerId);

        // Check for an error message in the response
        if (response.errorMessage != "")
        {
            Debug.LogError($"Error checking turn: {response.errorMessage}");
            turnStatusText.text = "";  // No turn data, so clear text
            return;
        }

        // Log whether it's the player's turn or not
        var isPlayerTurn = response.data;

        // Update UI based on whether it's the player's turn or not
        if (isPlayerTurn)
        {
            turnStatusText.text = "It's your turn!";
        }
        else
        {
            turnStatusText.text = "Waiting for the other player's turn...";
        }
    }

    // Check and set the targetPlayerId if 'FromPlayer' is set in the server-side storage
    private async Task CheckAndSetTargetPlayer()
    {
        var response = await _service.GetFromPlayer();

        if (string.IsNullOrEmpty(response.errorMessage))
        {
            var fromPlayerId = response.data;

            if (fromPlayerId != 0)
            {
                targetPlayerId = fromPlayerId;
            }
            else
            {
                Debug.Log("No 'FromPlayer' set, using default Target Player ID.");
            }
        }
        else
        {
            Debug.LogError($"Error fetching 'FromPlayer' data: {response.errorMessage}");
        }
    }
}
