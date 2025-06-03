using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common;
using Beamable.Server.Clients;
// using Beamable.Server.Clients;
using UnityEngine;

namespace DefaultNamespace
{
    public class StatsTest: MonoBehaviour
    {
        public BeamContext _beamContext;
        public long targetPlayerId = 1234567890;

        private async void Start()
        {
            _beamContext = await BeamContext.Default.Instance;
            await CallMicroservice(targetPlayerId);
        }

        private async Task CallMicroservice(long playerId)
        {
            try
            {
                var service = new ServiceClient(); 
                var result = await service.GetOtherPlayerStats(playerId);

                Debug.Log($"Stats Retrieved for {_beamContext.PlayerId}:");
                foreach (var kvp in result)
                {
                    Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
                }
            }
            catch (PromiseException e)
            {
                Debug.LogError($"Failed to get stats: {e.Message}");
            }
        }
    }
}
