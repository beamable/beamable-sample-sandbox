using System;
using System.Threading.Tasks;
using Beamable.Common.Api.Groups;
using Beamable.Server;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        private static readonly Random Random = new Random();

        [ClientCallable]
        public async Task<string> CreateGroupWithSpaces(string groupName)
        {
            var groupNameDifferentiator = Random.Next(1000).ToString();
            var fullGroupName = $"{groupName} {groupNameDifferentiator}";

            try
            {
                var groupCreateRequest = new GroupCreateRequest(fullGroupName, null, "open", 0, 50);
                var response = await Services.Groups.CreateGroup(groupCreateRequest);
                var groupId = response.group.id.ToString();
                return $"SUCCESS: Group '{fullGroupName}' created with ID: {groupId}";
            }
            catch (Exception e)
            {
                return $"ERROR: {e.Message}";
            }
        }
    }
}