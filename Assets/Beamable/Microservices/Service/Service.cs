using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [AdminOnlyCallable]
        public async Task SayHi()
        {
            try
            {
                Debug.Log("hi");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"too mean to say hi");
            }
        }
    }
}