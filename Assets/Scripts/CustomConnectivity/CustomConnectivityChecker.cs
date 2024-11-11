using System;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api;
using UnityEngine;
using UnityEngine.Networking;

namespace CustomConnectivity
{
    public class CustomConnectivityChecker : IConnectivityChecker
    {
        private float HeartbeatInterval { get; set; } = 5f;  // Customizable heartbeat interval
        public event Action<bool> OnConnectivityChanged;

        private bool _isConnected = true;
        private bool _disabled;

        public CustomConnectivityChecker()
        {
            StartHeartbeat();
        }

        public bool HasConnectivity => _isConnected && !_disabled;

        public bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;
                OnConnectivityChanged?.Invoke(HasConnectivity);
            }
        }

        private void CheckConnectivity()
        {
            // _isConnected = !Disabled;
            // OnConnectivityChanged?.Invoke(_isConnected);
            // if (_disabled)
            // {
            //     _isConnected = false;
            //     OnConnectivityChanged?.Invoke(false);
            //     return;
            // }

            // Attempt a simple network request to check connectivity
            using (var request = UnityWebRequest.Get("https://www.google.com"))
            {
                try
                {
                    request.SendWebRequest();
                    
                    // Determine connection status based on request result
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        if (!_isConnected)
                        {
                            _isConnected = true;
                            OnConnectivityChanged?.Invoke(true);
                        }
                    }
                    else
                    {
                        if (_isConnected)
                        {
                            _isConnected = false;
                            OnConnectivityChanged?.Invoke(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Connectivity check failed: " + ex.Message);
                    if (_isConnected)
                    {
                        _isConnected = false;
                        OnConnectivityChanged?.Invoke(false);
                    }
                }
            }
        }

        private async void StartHeartbeat()
        {
            while (true)
            {
                if (!Application.isPlaying)
                    return;
                
                await Task.Delay(TimeSpan.FromSeconds(HeartbeatInterval));
                CheckConnectivity();  
            }
        }
        

        public Promise<bool> ForceCheck()
        {
            var promise = new Promise<bool>();
            try
            {
                CheckConnectivity();  
                promise.CompleteSuccess(HasConnectivity);  // Return the current connectivity status
            }
            catch (Exception ex)
            {
                promise.CompleteError(ex);
            }
            return promise;
        }


        public bool ConnectivityCheckingEnabled { get; set; }
    }
}
