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
            Debug.Log("Service.GetComplexDictionary called");

            var fruitDict = new SerializableDictionary<string, int> { { "apple", 5 }, { "banana", 7 } };
            var veggieDict = new SerializableDictionary<string, int> { { "carrot", 3 }, { "date", 4 } };

            Debug.Log($"FruitDict keys: {string.Join(",", fruitDict.Keys)}");
            Debug.Log($"VeggieDict keys: {string.Join(",", veggieDict.Keys)}");
            fruitDict.OnBeforeSerialize();
            veggieDict.OnBeforeSerialize();
            var result = new ComplexDictionaryResult
            {
                entries = new List<CategoryEntry>
                {
                    new CategoryEntry
                    {
                        category = "fruits",
                        values = fruitDict
                    },
                    new CategoryEntry
                    {
                        category = "veggies",
                        values = veggieDict
                    }
                }
            };

            Debug.Log("Service.GetComplexDictionary returning result");

            return result;
        }
    }
}