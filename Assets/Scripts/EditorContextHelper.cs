using System;
using Beamable;
using Beamable.Common.Api.Auth;
using UnityEngine;

public static class EditorContextHelper
{
    public static BeamContext CreateIngameContext(this BeamEditorContext editor, string playerCode = "")
    {
        Debug.Log("Starting CreateIngameContext...");
    
        // Log the provided playerCode
        Debug.Log($"PlayerCode: {playerCode}");

        // Retrieve the token from the editor requester
        var tokenEditor = editor.Requester.Token;

        // Log token details
        Debug.Log($"Token Editor Details:");
        Debug.Log($"- Token: {tokenEditor.Token}");
        Debug.Log($"- Expires At: {tokenEditor.ExpiresAt}");
        Debug.Log($"- Refresh Token: {tokenEditor.RefreshToken}");

        // Calculate expires_in (time in seconds until expiration)
        var currentTime = DateTime.UtcNow;
        var expiresInSeconds = (long)(tokenEditor.ExpiresAt - currentTime).TotalSeconds;

        // Ensure expiresInSeconds is positive; fallback to a default if invalid
        if (expiresInSeconds < 0)
        {
            Debug.LogWarning("ExpiresIn is negative; using default value of 3600 seconds.");
            expiresInSeconds = 3600; // Default to 1 hour
        }

        // Create a token response
        var token = new TokenResponse
        {
            access_token = tokenEditor.Token,
            token_type = "access",
            expires_in = expiresInSeconds,
            refresh_token = tokenEditor.RefreshToken
        };

        // Log the generated token response
        Debug.Log($"Generated TokenResponse:");
        Debug.Log($"- Access Token: {token.access_token}");
        Debug.Log($"- Token Type: {token.token_type}");
        Debug.Log($"- Expires In (Seconds): {token.expires_in}");
        Debug.Log($"- Refresh Token: {token.refresh_token}");

        // Create and return the BeamContext
        try
        {
            var beamContext = BeamContext.CreateAuthorizedContext(playerCode, token);
            Debug.Log($"BeamContext successfully created with ID '{playerCode}'. Player ID: {beamContext.PlayerId}");
            return beamContext;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create BeamContext. Exception: {e.Message}");
            throw;
        }
    }

}