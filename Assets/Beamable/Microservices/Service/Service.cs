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
        private const long TurnDataId = 1111111111111;
        
        [ClientCallable]
        public async Task<Response<bool>> Ping(long playerId, long targetPlayerId)
        {
            try
            {
                // Fetch the turnData by playerId
                var turnData = await Storage.GetByFieldName<TurnData, long>("TurnDataId", TurnDataId);

                if (turnData == null)
                {
                    // Create TurnData if it doesn't exist
                    turnData = new TurnData()
                    {
                        TurnDataId = TurnDataId,
                        CurrentTurn = playerId
                    };
                    await Storage.Create<ServiceDataStorage, TurnData>(turnData);
                }

                // Check if it's the player's turn
                if (turnData.CurrentTurn != playerId)
                {
                    return new Response<bool>(false, "It's not your turn.");
                }

                // Switch turn to the other player and update turnData
                turnData.SwitchTurn(targetPlayerId);
                Debug.Log($"From player: {turnData.FromPlayer}");
                // Send the ping notification to the other player
                await SendPingNotification(turnData); // Pass both sender and target

                await Storage.Update(turnData.Id, turnData);

                return new Response<bool>(true);
            }
            catch (Exception e)
            {
                BeamableLogger.LogError(e);
                return new Response<bool>(false, "Error sending ping.");
            }
        }

        // Helper method to send a ping notification with senderId included
        private async Task SendPingNotification(TurnData turnData)
        {
            var playerIds = new List<long> { turnData.CurrentTurn };
            await Services.Notifications.NotifyPlayer(playerIds, "PingNotification", turnData);
        }

        [ClientCallable]
        public async Task<Response<bool>> IsPlayerTurn(long playerId)
        {
            try
            {
                // Fetch the TurnData by shared TurnDataId or session ID
                var turnData = await Storage.GetByFieldName<TurnData, long>("TurnDataId", TurnDataId);

                if (turnData == null)
                {
                    return new Response<bool>(false, "No turn data found.");
                }
                
                // Return true if it's the player's turn, otherwise false
                var isPlayerTurn = turnData.CurrentTurn == playerId;
                return new Response<bool>(isPlayerTurn);
            }
            catch (Exception e)
            {
                BeamableLogger.LogError(e);
                return new Response<bool>(false, "Error retrieving turn data.");
            }
        }

        [ClientCallable]
        public async Task<Response<long>> GetFromPlayer()
        {
            try
            {
                // Fetch TurnData from storage
                var turnData = await Storage.GetByFieldName<TurnData, long>("TurnDataId", TurnDataId);
                return turnData != null ? new Response<long>(turnData.FromPlayer) : new Response<long>(0, "No turn data found.");
            }
            catch (Exception e)
            {
                BeamableLogger.LogError(e);
                return new Response<long>(0, "Error retrieving FromPlayer.");
            }
        }
    }
}
