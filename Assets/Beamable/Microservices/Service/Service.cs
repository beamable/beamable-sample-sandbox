using System;
using System.Threading.Tasks;
using Beamable.Common.Api.Groups;
using Beamable.Server;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public async void GetCurrent()
        {
            // var result = await Services.P
        }
    }
}