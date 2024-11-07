using System;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api;
using UnityEngine;

    public class CustomConnectivityChecker : IConnectivityChecker
    {
        private float HeartbeatInterval { get; set; } = 10f;  // Customizable heartbeat interval
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
            _isConnected = !Disabled;
            OnConnectivityChanged?.Invoke(_isConnected);
        }

        private async void StartHeartbeat()
        {
            while (true)
            {
                if (!Application.isPlaying)
                    return;
                
                await Task.Delay(TimeSpan.FromSeconds(HeartbeatInterval));
                CheckConnectivity();  // Invoke connectivity check at custom intervals
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
