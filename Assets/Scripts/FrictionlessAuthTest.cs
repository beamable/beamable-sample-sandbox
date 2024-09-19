using UnityEngine;
using Beamable;
using Beamable.Player;

public class BeamableFrictionlessAuthTest : MonoBehaviour
{
    private BeamContext _beamContext;
    public PlayerAccount account;

    private async void Start()
    {
        // Initialize BeamContext
        _beamContext = BeamContext.Default;

        try
        {
            // Await Beamable to be ready
            await _beamContext.OnReady;
            Debug.Log("Beamable SDK initialized successfully.");

            // Await the Accounts service to be ready
            await _beamContext.Accounts.OnReady;
            account = _beamContext.Accounts.Current;

            // Log PlayerId
            if (account != null && !string.IsNullOrEmpty(account.GamerTag.ToString()))
            {
                Debug.Log($"PlayerId is valid: {account.GamerTag}");
            }
            else
            {
                Debug.LogError("PlayerId is invalid or empty.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error during Beamable initialization: {ex.Message}");
        }
    }
}