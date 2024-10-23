using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Models;
using Beamable.Common.Utils;
using Beamable.Mongo;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        
         // Method to check if a device is authorized
        [ClientCallable]
        public async Task<Response<bool>> CheckDeviceAuthorization(string email, string deviceId)
        {
            try
            {
                // Fetch the device data for the player
                var deviceData = await Storage.GetByFieldName<AuthorizedDeviceData, string>("Email", email);
                
                if (deviceData == null || !deviceData.AuthorizedDeviceIds.Contains(deviceId))
                {
                    return new Response<bool>(false, "Unauthorized device.");
                }

                return new Response<bool>(true, "Device is authorized.");
            }
            catch (Exception e)
            {
                BeamableLogger.LogError(e);
                return new Response<bool>(false, "Error checking device authorization.");
            }
        }

        // Method to authorize a new device for a player
        [ClientCallable]
        public async Task<Response<bool>> AuthorizeDevice(string email, string deviceId)
        {
            Debug.Log("sfe");
            try
            {
                // Fetch or create device data for the player
                var deviceData = await Storage.GetByFieldName<AuthorizedDeviceData, string>("PlayerId", email);

                if (deviceData == null)
                {
                    // Create a new entry if no data exists for the player
                    deviceData = new AuthorizedDeviceData()
                    {
                        Email = email,
                        AuthorizedDeviceIds = new List<string> { deviceId }
                    };
                    await Storage.Create<ServiceDataStorage, AuthorizedDeviceData>(deviceData);
                }
                else
                {
                    // Add deviceId to the list if it's not already authorized
                    if (!deviceData.AuthorizedDeviceIds.Contains(deviceId))
                    {
                        deviceData.AuthorizedDeviceIds.Add(deviceId);
                        await Storage.Update(deviceData.Id, deviceData);
                    }
                }

                return new Response<bool>(true, "Device authorized.");
            }
            catch (Exception e)
            {
                BeamableLogger.LogError(e);
                return new Response<bool>(false, "Error authorizing device.");
            }
        }

        // Method to reset device authorizations for a player (used after password reset)
        [ClientCallable]
        public async Task<Response<bool>> ResetDeviceAuthorizations(string email)
        {
            try
            {
                // Fetch the player's device data
                var deviceData = await Storage.GetByFieldName<AuthorizedDeviceData, string>("PlayerId", email);

                if (deviceData != null)
                {
                    // Clear the list of authorized devices
                    deviceData.AuthorizedDeviceIds.Clear();
                    await Storage.Update(deviceData.Id, deviceData);
                }

                return new Response<bool>(true, "Device authorizations reset.");
            }
            catch (Exception e)
            {
                BeamableLogger.LogError(e);
                return new Response<bool>(false, "Error resetting device authorizations.");
            }
        }
    }
}
