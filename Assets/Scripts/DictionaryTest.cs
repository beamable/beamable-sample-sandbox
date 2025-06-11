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
            Debug.Log("Initializing BeamContext...");
            _beamContext = await BeamContext.Default.Instance;

            Debug.Log("BeamContext initialized. Calling microservice...");
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
                    
                    if (entry.values == null)
                    {
                        Debug.LogWarning("Entry values are null");
                        continue;
                    }
                    
                    entry.values.OnAfterDeserialize();

                    try
                    {
                        int count = 0;
                        foreach (var kv in entry.values)
                        {
                            Debug.Log($"[entry.values] Key #{count + 1}: {kv.Key}, Value: {kv.Value}");
                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Exception while iterating entry.values: {ex}");
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
