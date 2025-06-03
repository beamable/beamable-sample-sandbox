using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api.Groups;
using Beamable.Server;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public async Task<Dictionary<string, string>> GetOtherPlayerStats(long playerId)
        {
            var otherUser = AssumeUser(playerId, false);
            string domain = "game";
            string access = "private";
            string type = "player";
            string[] keys = null; // null means all stats

            var stats = await otherUser.Services.Stats.GetStats(domain, access, type, playerId, keys);

            return stats;
        }
    }
}