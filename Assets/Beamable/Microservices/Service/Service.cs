using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Models;
using Beamable.Common.Utils;
using Beamable.Mongo;
using Beamable.Server;
using Beamable.Server.Api.Leaderboards;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
  private const int EventDurationHours = 4;

        [ClientCallable]
        public async Task<Response<EventData>> CreateEvent(string eventName)
        {
            try
            {
                var leaderboardId = $"leaderboard_{eventName}_{DateTime.UtcNow.Ticks}";

                // Create the leaderboard for this event
                await Services.Leaderboards.CreateLeaderboard(leaderboardId, new CreateLeaderboardRequest());

                // Define start and end times for the event
                DateTime startTime = DateTime.UtcNow;
                DateTime endTime = startTime.AddHours(EventDurationHours);

                // Create event data and save to storage
                var eventData = new EventData
                {
                    eventName = eventName,
                    leaderboardId = leaderboardId,
                    startTime = startTime,
                    endTime = endTime,
                    isActive = true
                };
                
                await Storage.Create<ServiceDataStorage, EventData>(eventData);

                Debug.Log($"Event '{eventName}' created with leaderboard '{leaderboardId}'");
                return new Response<EventData>(eventData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating event: {e.Message}");
                return new Response<EventData>(null, "Error creating event");
            }
        }

        [ClientCallable]
        public async Task<Response<bool>> SubmitScore(string eventId, double score)
        {
            try
            {
                var eventData = await Storage.Get<ServiceDataStorage, EventData>(eventId);
                if (eventData is not { isActive: true })
                {
                    Debug.LogError("Event not found or not active.");
                    return new Response<bool>(false, "Event not found or not active.");
                }

                // Submit score to the event's leaderboard
                await Services.Leaderboards.SetScore(eventData.leaderboardId, score);
                Debug.Log($"Score {score} submitted to leaderboard '{eventData.leaderboardId}'");
                return new Response<bool>(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error submitting score: {e.Message}");
                return new Response<bool>(false, "Error submitting score");
            }
        }

        [ClientCallable]
        public async Task<Response<EventData>> CheckEventStatus(string eventId)
        {
            try
            {
                var eventData = await Storage.Get<ServiceDataStorage, EventData>(eventId);
                if (eventData == null)
                {
                    return new Response<EventData>(null, "Event not found");
                }

                // Check if event has ended and mark as inactive if necessary
                if (DateTime.UtcNow < eventData.endTime || !eventData.isActive)
                    return new Response<EventData>(eventData);
                
                eventData.isActive = false;
                await Storage.Update(eventData.Id, eventData);
                Debug.Log($"Event '{eventData.eventName}' marked as inactive.");

                return new Response<EventData>(eventData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error checking event status: {e.Message}");
                return new Response<EventData>(null, "Error checking event status");
            }
        }

        [ClientCallable]
        public async Task<Response<bool>> ClaimReward(string eventId, string itemRef, string itemAmountProperty, long amount)
        {
            try
            {
                var eventData = await Storage.Get<ServiceDataStorage, EventData>(eventId);
                if (eventData == null || eventData.isActive)
                {
                    return new Response<bool>(false, "Event not found or still active.");
                }

                // Reward logic based on leaderboard rank or score can be implemented here
                var itemProperties = new Dictionary<string, string>
                {
                    { itemAmountProperty, amount.ToString() }
                };
                await Services.Inventory.AddItem(itemRef, itemProperties);
                Debug.Log($"Reward claimed for event '{eventData.eventName}'");
                return new Response<bool>(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error claiming reward: {e.Message}");
                return new Response<bool>(false, "Error claiming reward");
            }
        }

        [ClientCallable]
        public async Task<Response<EventData>> GetActiveEvent(string eventId)
        {
            try
            {
                var eventData = await Storage.GetByFieldName<EventData, bool>("isActive", true);
                
                return new Response<EventData>(eventData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error retrieving active events: {e.Message}");
                return new Response<EventData>(null, "Error getting active event");
            }
        }
    }
}
