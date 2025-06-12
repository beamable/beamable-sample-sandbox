using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common;
using Beamable.Server.Clients;
using UnityEngine;

namespace DefaultNamespace
{
    public class DictionaryTest : MonoBehaviour
    {
        public BeamContext _beamContext;

        private async void Start()
        {
            _beamContext = await BeamContext.Default.Instance;

            await CallMicroservice();
        }

        private async Task CallMicroservice()
        {
            try
            {
                var service = _beamContext.Microservices().Service();
                var result = await service.GetComplexDictionary();

                if (result == null || result.entries == null)
                {
                    Debug.LogWarning("Result or entries is null!");
                    return;
                }

                foreach (var entry in result.entries)
                {
                    Debug.Log($"Category: {entry.category}");
                    foreach (var kv in entry.values)
                    {
                        Debug.Log($"Key: {kv.key}, Value: {kv.value}");
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to get complex stats: {e}");
            }
        }
    }
}