using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api.Groups;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public async Task SetIsVipStat(long playerId)
        {
            string statKey = "is_vip";
            string access = "public";
            string newValue = "true";

            Dictionary<string, string> stats = new Dictionary<string, string>
            {
                { statKey, newValue }
            };

            await Services.Stats.SetStats(access, stats);
            Debug.Log($"Updated '{statKey}' for Player {playerId}.");
        }
        
    }
}