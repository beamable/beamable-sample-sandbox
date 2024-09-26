using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;

public class DeepLinkExample : MonoBehaviour
{
    void Start()
    {
        // For testing in the Unity Editor, mock a deep link URL with a referral ID
#if UNITY_EDITOR
        string mockUrl = "https://yourgame.com?referrer=9999999";
        HandleDeepLink(mockUrl);
#endif

        // Capture deep link if the app was launched via deep link
        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            HandleDeepLink(Application.absoluteURL);
        }

        // Subscribe to future deep link activations
        Application.deepLinkActivated += HandleDeepLink;
    }

    void HandleDeepLink(string url)
    {
        // Parse the referrer ID from the URL
        Uri uri = new Uri(url);
        var query = uri.Query;
        var referralId = System.Web.HttpUtility.ParseQueryString(query).Get("referrer");

        // Mock fallback for testing in case the referral ID is null
        if (string.IsNullOrEmpty(referralId))
        {
            referralId = "empty"; // Mocked referrer ID for testing
        }

        Debug.Log("Referral ID: " + referralId);

        // Call your method to handle the referral in Beamable
        // SendReferralToBeamable(referralId);
    }

}
