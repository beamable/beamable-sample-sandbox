using System;
using System.Threading.Tasks;
using Beamable;
using Beamable.Server.Clients;
using DefaultNamespace;
using UnityEngine;

public class UserLoginManager
{
    private BeamContext _ctx;

    public async Task RegisterOrLoginUser(string token, bool isNewUser)
    {
        _ctx = await BeamContext.Default.Instance;

        // Add external identity (Federated Identity)
        try
        {
            Debug.Log(_ctx.PlayerId);
            await _ctx.Accounts.AddExternalIdentity<KoinIdentity, ServiceClient>(token);
            Debug.Log("External Identity added successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error adding external identity: {ex.Message}");
        }
    } 
}
