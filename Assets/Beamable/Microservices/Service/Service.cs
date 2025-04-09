using System;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api;
using Beamable.Common.Api.Groups;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public async Promise<bool> SendGroupInvite(long gamerTag, long groupId)
        {
            var request = new GroupInviteRequest
            {
                gamerTag = gamerTag,
                subGroup = "", // Or actual value
                useNewRewardsSystem = false
            };

            try
            {
                var result = await Requester.Request(
                    Method.POST,
                    $"object/groups/{groupId}/invite",
                    body: request,
                    parser: s => s
                );

                Debug.Log("[SendGroupInvite] Beamable group invite success");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SendGroupInvite] Beamable group invite failed: {e.Message}");
                return false;
            }
        }
        [Serializable]
        public class GroupInviteRequest
        {
            public long gamerTag;
            public string subGroup;
            public bool useNewRewardsSystem;
        }

    }
}