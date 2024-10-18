using System;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Groups;
using Beamable.Server;
using DefaultNamespace;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice, IFederatedLogin<KoinIdentity>
    {
        public async Promise<FederatedAuthenticationResponse> Authenticate(string token, string challenge, string solution)
        {
            // Simulate user validation
            var userResponse = await KoinService.GetUserByAuthorizationCode(token);
            if (userResponse == null)
            {
                throw new Exception();
            }
            return new FederatedAuthenticationResponse { user_id = userResponse.userId };
        } 
    }
}