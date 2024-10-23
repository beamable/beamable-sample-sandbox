using System;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Utils;
using TMPro;
using UnityEngine;
using Beamable.Server.Clients;

public class DeviceCheck : MonoBehaviour
{
    private ServiceClient _service; 
    private BeamContext _beamContext;

    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TextMeshProUGUI statusText;

    private async void Start()
    {
        try
        {
            // Initialize ServiceClient and BeamContext
            _service = new ServiceClient();
            _beamContext = await BeamContext.Default.Instance;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error initializing: {ex.Message}");
        }
    }

    // Method to check if the current device is authorized for the player's account (based on email)
    private async Task<Response<bool>> CheckDeviceAuthorization(string deviceId)
    {
        try
        {
            var checkResponse = await _service.CheckDeviceAuthorization(email.text, deviceId);
            return checkResponse;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during device authorization check: {ex.Message}");
            return new Response<bool>(false, "Error checking device authorization.");
        }
    }

    // Method to authorize the current device after a successful login
    private async Task<bool> AuthorizeDevice(string deviceId)
    {
        try
        {
            var response = await _service.AuthorizeDevice(email.text, deviceId);
            return response.data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during device authorization: {ex.Message}");
            return false;
        }
    }

    // Add email, password, and device to the existing account during signup
    private async Task Signup()
    {
        var isEmailAvailable = await _beamContext.Accounts.IsEmailAvailable(email.text);
        var deviceId = SystemInfo.deviceUniqueIdentifier; // Get the device ID

        if (!isEmailAvailable)
        {
            // Email is taken, attempt to log in
            var loginResult = await LoginUser();
            if (!loginResult)
            {
                // Login failed; show message indicating that the email is taken
                statusText.text = "Email is already taken. Please check your password or use a different email."; // UI feedback
            }
        }
        else
        {
            // Proceed with sign-up if email is available
            var result = await _beamContext.Accounts.AddEmail(email.text, password.text);
            if (!result.isSuccess)
            {
                statusText.text = $"Signup failed: {result.error}"; 
            }
            else
            {
                Debug.Log("Email added successfully.");
                statusText.text = "Signup successful!"; 

                // Authorize the device after a successful signup
                var isDeviceAuthorized = await AuthorizeDevice(deviceId);

                if (!isDeviceAuthorized)
                {
                    Debug.LogError("Failed to authorize device.");
                    statusText.text = "Signup successful, but device authorization failed. Please contact support.";
                }
                else
                {
                    Debug.Log("Device authorized successfully after signup.");
                }
            }
        }
    }

    // Login user with email, password, and verify device
    private async Task<bool> LoginUser()
    {
        var deviceId = SystemInfo.deviceUniqueIdentifier;

        // Check if the current device is authorized for this email account before logging in
        var isDeviceAuthorized = await CheckDeviceAuthorization(deviceId);

        if (!isDeviceAuthorized.data)
        {
            statusText.text = "Unauthorized device. Please contact support.";
            return false;
        }

        // If the device is authorized, attempt to recover the account
        var operation = await _beamContext.Accounts.RecoverAccountWithEmail(email.text, password.text);
        if (operation.isSuccess)
        {
            // Player successfully logged in
            operation.SwitchToAccount();
            statusText.text = "Login successful!";
            return true;
        }

        statusText.text = $"Login failed: {operation.error}";
        return false;
    }


    // Called when the login button is clicked
    public async void OnLoginButtonClicked()
    {
        Debug.Log("Login button clicked.");
        await LoginUser();
    }

    // Called when the signup button is clicked
    public async void OnSignUpButtonClicked()
    {
        Debug.Log("Signup button clicked.");
        await Signup();
    }
}
