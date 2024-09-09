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
            // Initialize the Beamable context
            _beamContext = await BeamContext.Default.Instance;

            // Initialize the microservice client
            _service = new ServiceClient();
            
            // Call the method from the microservice
            await _service.A(_beamContext.PlayerId);

            Debug.Log("Triggered microservice method A.");
        }
    }
}