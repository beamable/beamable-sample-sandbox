using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Auth;
using Beamable.Common.Dependencies;
using DefaultNamespace;

public class KoinService : ISupportsFederatedLogin<KoinIdentity>
{
    // Mock user database
    private static Dictionary<string, UserResponse> _userDatabase = new Dictionary<string, UserResponse>
    {
        { "user1_token", new UserResponse { userId = "user1_id" } },
        { "user2_token", new UserResponse { userId = "user2_id" } }
    };

    // Simulate fetching user details by authorization code
    public static Task<UserResponse> GetUserByAuthorizationCode(string token)
    {
        // Check if the token exists in the mock database
        if (_userDatabase.TryGetValue(token, out var userResponse))
        {
            return Task.FromResult(userResponse);
        }
        else
        {
            return Task.FromResult<UserResponse>(null); // No user found for this token
        }
    }

    // Implement ServiceName from IHaveServiceName
    public string ServiceName => "KoinService";

    // Implement Provider from ISupportsFederatedLogin
    private IDependencyProvider _provider;
    public IDependencyProvider Provider
    {
        get => _provider;
        set => _provider = value;
    }

    // Implement Authenticate method from ISupportsFederatedLogin
    public async Promise<FederatedAuthenticationResponse> Authenticate(string token, string challenge, string solution)
    {
        var userResponse = await GetUserByAuthorizationCode(token);
        if (userResponse == null)
        {
            throw new Exception();
        }
        return new FederatedAuthenticationResponse { user_id = userResponse.userId };
    }
}