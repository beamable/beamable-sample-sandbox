using System;
using System.Threading.Tasks;
using Beamable.Common.Api.Groups;
using Beamable.Server;
using Beamable.Server.Api.Leaderboards;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public async Task SetLeaderboardScore(string leaderboardId, double score)
        {
            try
            {
                await Services.Leaderboards.SetScore(leaderboardId, score);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }
        
        [ClientCallable]
        public async Task CreateLeaderboard(string leaderboardId)
        {
            try
            {
                await Services.Leaderboards.CreateLeaderboard(leaderboardId, new CreateLeaderboardRequest());
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }
    }
}