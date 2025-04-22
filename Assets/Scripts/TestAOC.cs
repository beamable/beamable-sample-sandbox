using Beamable;
using Beamable.Server.Clients;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class TestAOC : MonoBehaviour
{
    private BeamEditorContext _adminContext;
    private BeamContext _beamContext;
    private ServiceClient _service;

    private const string StatKey = "vip_flag";
    private const string Access = "public";
    private const string Domain = "client";
    private const string Type = "player";

    private async void Start()
    {
        try
        {
            Debug.Log("Initializing Admin Context...");
            _adminContext = BeamEditorContext.Default;

            if (_adminContext == null)
            {
                Debug.LogError("Failed to initialize BeamEditorContext.Default.");
                return;
            }

            Debug.Log("Admin Context successfully initialized.");
            Debug.Log($"Admin Context Player ID: {_adminContext.EditorAccount.cid}");

            Debug.Log("Creating in-game context...");
            _beamContext = _adminContext.CreateIngameContext("admin");

            if (_beamContext == null)
            {
                Debug.LogError("Failed to create in-game context.");
                return;
            }

            Debug.Log("In-game context successfully created.");
            Debug.Log($"In-game context Player ID: {_beamContext.PlayerId}");

            Debug.Log("Initializing Service Client...");
            _service = _beamContext.Microservices().Service();

            if (_service == null)
            {
                Debug.LogError("Failed to initialize Service Client.");
                return;
            }

            Debug.Log("Service Client successfully initialized.");


            Debug.Log("Calling MS");
            await _service.SayHi();
            Debug.Log("MS call works");

        }
        catch (System.ArgumentOutOfRangeException ex)
        {
            Debug.LogError($"Argument out of range: {ex.Message} - Parameter: {ex.ParamName}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error during stat operation: {ex.Message}");
        }
    }

    [MenuItem("Beamable/Admin/Say Hi")]
    public static async void SetVipStat()
    {
        try
        {
            // Use BeamEditorContext for admin scope
            var editorContext = BeamEditorContext.Default;

            // Use the helper to create an in-game context based on admin context
            var beamContext = editorContext.CreateIngameContext("adminContextMenu");
            var service = beamContext.Microservices().Service();

            await service.SayHi();

            Debug.Log("MS Said Hi");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in MenuItem call: {ex.Message}");
        }
    }
}
