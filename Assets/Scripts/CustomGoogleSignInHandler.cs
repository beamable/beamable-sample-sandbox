using Beamable.Common.Api.Auth;
using System.Threading.Tasks;
using Beamable;
using Beamable.AccountManagement;
using Beamable.Platform.SDK.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomGoogleSignInHandler : MonoBehaviour
{
    private BeamContext _beamContext;
    private string webClientId = "604698301882-14l582parnqn63ha125n27kaon1red41.apps.googleusercontent.com";
    private GoogleSignIn _google;

    private async void Start()
    {
        Debug.Log("Initializing BeamContext...");
        _beamContext = await BeamContext.Default.Instance;
        Debug.Log("BeamContext initialized.");
    }

    /// <summary>
    /// Starts the Google Sign-In process.
    /// </summary>
    public void StartGoogleLogin()
    {
        Debug.Log("Starting Google Sign-In process...");
        _google = new GoogleSignIn(gameObject, "GoogleAuthResponse", webClientId, "");
        _google.Login();
        GoogleAuthResponse("GoogleAuthResponse");
    }

    /// <summary>
    /// Callback to be invoked via UnitySendMessage when the plugin either
    /// receives a valid ID token or indicates an error.
    /// </summary>
    /// <param name="message">Response message from the Google Sign-In plugin</param>
    private async void GoogleAuthResponse(string message)
    {
        Debug.Log("Received Google Auth response.");
        GoogleSignIn.HandleResponse(message, async token =>
        {
            if (token == null)
            {
                Debug.LogError("Login failed or was cancelled");
                return;
            }

            Debug.Log("Token received, proceeding with login flow...");

            var thirdParty = AuthThirdParty.Google;
            var available = await _beamContext.Api.AuthService.IsThirdPartyAvailable(thirdParty, token);
            var userHasCredentials = _beamContext.Api.User.HasThirdPartyAssociation(thirdParty);

            if (!available)
            {
                Debug.Log("User not available, switching users...");
                await _beamContext.Api.AuthService.LoginThirdParty(thirdParty, token, false);
                ChangeScreen("UserSwitchScreen");
            }
            else if (userHasCredentials)
            {
                Debug.Log("Attaching to current user...");
                var user = await _beamContext.Api.AuthService.RegisterThirdPartyCredentials(thirdParty, token);
                _beamContext.Api.UpdateUserData(user);
                ChangeScreen("MainMenuScreen");
            }
            else
            {
                Debug.Log("Creating new user...");
                var tokenResponse = await _beamContext.Api.AuthService.CreateUser();
                _beamContext.Api.ApplyToken(tokenResponse);
                var user = await _beamContext.Api.AuthService.RegisterThirdPartyCredentials(thirdParty, token);
                _beamContext.Api.UpdateUserData(user);
                ChangeScreen("UserCreationScreen");
            }
        },
        errback =>
        {
            Debug.LogError($"Error during Google Sign-In: {errback}");
        });
    }

    /// <summary>
    /// Changes the screen to the specified screen name.
    /// </summary>
    /// <param name="screenName">The name of the screen to load.</param>
    private void ChangeScreen(string screenName)
    {
        Debug.Log($"Changing to screen: {screenName}");
        SceneManager.LoadScene("NewScene");
    }
}
