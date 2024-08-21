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
        public async Task ResetStats(string statKey)
        {
            const string access = "public";
            const string newValue = "1";
            
            Dictionary<string, string> resetStats = new Dictionary<string, string>() 
            {
                { statKey, newValue }
            };

            await Services.Stats.SetStats(access, resetStats);
        }
    }
}