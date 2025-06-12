using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Groups;
using Beamable.Common.Content;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public async Task<ComplexDictionaryResult> GetComplexDictionary()
        {
            var fruitValues = new List<EntryValue>
            {
                new EntryValue { key = "apple", value = 5 },
                new EntryValue { key = "banana", value = 7 }
            };

            var veggieValues = new List<EntryValue>
            {
                new EntryValue { key = "carrot", value = 3 },
                new EntryValue { key = "date", value = 4 }
            };

            foreach (var kv in fruitValues)
            {
                Debug.Log($"Fruit - Key: {kv.key}, Value: {kv.value}");
            }

            foreach (var kv in veggieValues)
            {
                Debug.Log($"Veggie - Key: {kv.key}, Value: {kv.value}");
            }

            var result = new ComplexDictionaryResult
            {
                entries = new List<CategoryEntry>
                {
                    new CategoryEntry { category = "fruits", values = fruitValues },
                    new CategoryEntry { category = "veggies", values = veggieValues }
                }
            };

            foreach (var entry in result.entries)
            {
                Debug.Log($"Category: {entry.category}, Entry count: {entry.values?.Count ?? 0}");
            }

            return result;
        }
    }
}