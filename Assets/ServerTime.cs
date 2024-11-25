using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Beamable;
using Beamable.Api;
using Beamable.Common.Api;
using Beamable.Common.Dependencies;
using UnityEditor;
using UnityEngine;

public class ServerTime : MonoBehaviour
{
    private const string Prefix = "editor.0";
    private const string Cid = "1774561898395648";
    private const string Pid = "DE_1776244147362865";
    
    private static string PrefsKey(string thing)
    {
    	return $"{Prefix}.{Cid}.{Pid}.{thing}";
    }
    
    private static string CidPrefsKey(string thing)
    {
    	return $"{Prefix}.{Cid}.{thing}";
    }
    
    [MenuItem("DEBUG/Show Token Info")]
    public static void ShowTokenExpiration()
    {
    	var accessToken = PlayerPrefs.GetString(CidPrefsKey("access_token"));
    	var expires = PlayerPrefs.GetString(CidPrefsKey("expires"));
    	var refreshToken = PlayerPrefs.GetString(CidPrefsKey("refresh_token"));
    	var expiresDate = "invalid date";
    	if (long.TryParse(expires, out var windowsFileTime))
    	{
    		expiresDate = DateTime.FromFileTimeUtc(windowsFileTime).ToString(CultureInfo.InvariantCulture);
    	}
    	Debug.Log($"Beamable token info: '{accessToken}' expiry {expires} ({expiresDate}) refresh='{refreshToken}'");
    }
    
    [MenuItem("DEBUG/Force Expire Token")]
    public static void ForceTokenExpiration()
    {
    	var expires = "0";
    	var prefsKey = CidPrefsKey("expires");
    	PlayerPrefs.SetString(prefsKey, expires);
    	Debug.Log("Beamable token expiration set to epoch.");
    }
    
    [MenuItem("DEBUG/Overwrite Access Token")]
    public static void OverwriteAccessToken()
    {
    	var token = "6af40274-cc4b-4010-b85e-acde7dfa5777";
    	var prefsKey = CidPrefsKey("access_token");
    	PlayerPrefs.SetString(prefsKey, token);
    	Debug.Log($"Overwrote access token with '{token}'");
    }
}