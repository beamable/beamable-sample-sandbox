using System.Collections.Generic;
using Beamable;
using Beamable.Server.Clients;
using UnityEngine;

public class DeleteAccounts : MonoBehaviour
{
    private BeamContext _beamContext;

    async void Start()
    {
        _beamContext = await BeamContext.Default.Instance;

        Debug.Log($"Current PlayerId: {_beamContext.PlayerId}");
        Debug.Log($"Cid: {_beamContext.Cid}");
        Debug.Log($"Pid: {_beamContext.Pid}");
        var deviceID = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Device Unique Identifier: " + deviceID);
    }
}