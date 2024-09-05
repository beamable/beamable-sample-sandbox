using System;
using System.Collections.Generic;
using Beamable;
using Beamable.Server.Clients;
using UnityEngine;

namespace DefaultNamespace
{
    public class NotificationTest: MonoBehaviour
    {
        private ServiceClient _service;
        private BeamContext _beamContext;

        private async void Start()
        {
            _beamContext = await BeamContext.Default.Instance;
            _service = new ServiceClient();
            
            // Define the payload you want to send with the notification
            var payload = new { invitation = "invite" };

            // Call the method from the microservice with the correct payload
            await _service.TriggerNotificationWithDelay(new List<long> { _beamContext.PlayerId }, "TestNotification", payload);
        }
    }
}