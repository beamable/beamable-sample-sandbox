using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Models;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public async Task<HarvestResponseDto> Harvest(int slotID)
        {
            try
            {
                // Simulate some processing
                Debug.Log("Processing Harvest...");
        
                var response = HarvestResponseDto.CreateSuccess(100);
                Debug.Log("Harvest complete.");

                return response;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return HarvestResponseDto.CreateError("Something went wrong!", slotID);
            }
        }
    }
}