using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public string TriggerNotificationWithDelay(List<long> playerIds, string messageContext, object payload)
        {
            // Perform some main task here (e.g., updating the player's status)
            Debug.Log("Doing some work...");

            // Fire-and-forget logic (task that runs asynchronously without being awaited)
            _ = Task.Run(async () =>
            {
                // Simulate a delay in processing (long-running task)
                await Task.Delay(5000); // Simulate 5-second delay

                // This should trigger the ServiceScopeDisposedException
                try
                {
                    var json = JsonUtility.ToJson(payload);
                    // Try to access Services after the delay (when the scope has been disposed)
                    await Services.Notifications.NotifyPlayer(playerIds, messageContext, json);
                    Debug.Log("Notification sent successfully");
                }
                catch (Exception e)
                {
                    Debug.Log($"Error occurred: {e.Message}");
                }
            });

            // Return immediately without awaiting the notification task
            return "Notification will be attempted after delay.";
        }
    }
}