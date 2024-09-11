using System;
using Beamable.Server.Clients;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestClass : MonoBehaviour
    {
        private ServiceClient _service;

        private async void Start()
        {
            _service = new ServiceClient();
            try
            {
                var result = await _service.Harvest(123456);
                Debug.Log("Result: " + result);
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception caught: " + ex.Message);
            }
        }
    }
}