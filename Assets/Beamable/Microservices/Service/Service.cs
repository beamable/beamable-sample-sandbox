using System;
using System.Threading.Tasks;
using Beamable.Common.Api.Groups;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public void Test()
        {
            Debug.Log("hello world");
        }
    }
}