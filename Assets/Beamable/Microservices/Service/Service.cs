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
        public Task A(long userId)
        {
            // update Inventory
            // Implementing fire-and-forget pattern with a discard (_) to avoid awaiting Notify.
            
            Debug.Log("calling notify");
            Task.Run(() => Notify(userId));
            Debug.Log("done");
            return Task.CompletedTask;
        }

        private async Task Notify(long userId)
        {
            try
            {
                var questUpdate = new { message = "Quest Updated!" };
                await Services.Notifications.NotifyPlayer(userId, "QUEST_UPDATE", questUpdate);
                Debug.Log("Player notified");
            }
            catch (Exception ex)
            {
                // Handle potential ServiceScopeDisposedException or other exceptions
                Debug.Log($"Error occurred during notification: {ex.Message}");
            }
        }

    }

}