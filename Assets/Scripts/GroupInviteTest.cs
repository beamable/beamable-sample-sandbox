using System;
using Beamable;
using Beamable.Common.Api.Groups;
using Beamable.Server.Clients;
using UnityEngine;

namespace DefaultNamespace
{
    public class GroupInviteTest: MonoBehaviour
    {
        private BeamContext _beamContext;
        private ServiceClient _serviceClient;
        private async void Start()
        {
            _beamContext = await BeamContext.Default.Instance;
            _serviceClient = new ServiceClient();
            Debug.Log(_beamContext.PlayerId);

        }

        public async void SendGroupInvite()
        {
            var result = await _serviceClient.SendGroupInvite(1857995730141185, 1864385994612684);
            Debug.Log(result);
        }

        public async void AcceptGroupInvite()
        {
            await _beamContext.Api.GroupsService.JoinGroup(1864385994612684);
            Debug.Log("joined group");
        }
    }
}