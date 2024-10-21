using Beamable;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BeamableLogin : MonoBehaviour
{
    private BeamContext _beamContext;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TextMeshProUGUI statusText;

    private async void Start()
    {
        // Initialize BeamContext
        _beamContext = await BeamContext.Default.Instance;
        await _beamContext.Accounts.OnReady;
        Debug.Log(_beamContext.PlayerId);
    }

    // Add email and password to the existing account
    private async Task Signup()
    {
        var isEmailAvailable = await _beamContext.Accounts.IsEmailAvailable(email.text);

        if (!isEmailAvailable)
        {
            // Email is taken, attempt to log in
            Debug.Log("Email already taken, attempting login...");
            var loginResult = await LoginUser();
            if (!loginResult)
            {
                // Login failed; show message indicating that the email is taken
                Debug.LogError("Login failed. The email is already registered, but the password may be incorrect.");
                statusText.text = "Email is already taken. Please check your password or use a different email."; // UI feedback
            }
        }
        else
        {
            // Proceed with sign-up if email is available
            var result = await _beamContext.Accounts.AddEmail(email.text, password.text);
            if (!result.isSuccess)
            {
                Debug.LogError($"Failed to add email, reason=[{result.error}]");
                statusText.text = $"Signup failed: {result.error}"; // UI feedback
            }
            else
            {
                Debug.Log("Email added successfully.");
                statusText.text = "Signup successful!"; // UI feedback
            }
        }
    }

    private async Task<bool> LoginUser()
    {
        var operation = await _beamContext.Accounts.RecoverAccountWithEmail(email.text, password.text);
        if (operation.isSuccess)
        {
            // Player ID from Game2 will be linked to the existing Account ID from Game1
            Debug.Log($"Found existing account, playerId=[{operation.account.GamerTag}]");
            operation.SwitchToAccount();
            Debug.Log("Switched to the recovered account successfully.");
            statusText.text = "Login successful!"; 
            return true;
        }

        Debug.LogError($"Failed to recover account via email, reason=[{operation.error}]");
        statusText.text = $"Login failed: {operation.error}"; 
        return false;
    }

    // Called when the login button is clicked
    public async void OnLoginButtonClicked()
    {
        await LoginUser();
    }

    // Called when the signup button is clicked
    public async void OnSignUpButtonClicked()
    {
        await Signup();
    }
}
