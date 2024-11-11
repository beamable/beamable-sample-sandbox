using System;
using System.Threading.Tasks;
using Beamable.Api.Connectivity;
using Beamable.Common;
using CustomConnectivity;
using UnityEngine;

namespace DefaultNamespace
{
    public class CustomConnectivityService : IConnectivityService
    {
        private readonly CustomConnectivityChecker _connectivityChecker;
        public bool Disabled { get; }
        public event Action<bool> OnConnectivityChanged;

        public CustomConnectivityService()
        {
            _connectivityChecker = new CustomConnectivityChecker();
            _connectivityChecker.OnConnectivityChanged += ConnectivityChangedHandler;
            
            // Setup diagnostic logging for unhandled exceptions
            Application.logMessageReceived += HandleLog;
        }

        public bool HasConnectivity => _connectivityChecker.HasConnectivity;

        public bool ForceDisabled
        {
            get => _connectivityChecker.Disabled;
            set => _connectivityChecker.Disabled = value;
        }

        public void OnReconnectOnce(ConnectionCallback promise, int order = 0)
        {
            try
            {
                if (HasConnectivity)
                {
                    promise();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error in OnReconnectOnce: " + ex.Message);
            }
        }

        private void ConnectivityChangedHandler(bool hasConnectivity)
        {
            try
            {
                OnConnectivityChanged?.Invoke(hasConnectivity);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error in ConnectivityChangedHandler: " + ex.Message);
            }
        }

        public Promise SetHasInternet(bool hasInternet)
        {
            var promise = new Promise();
            try
            {
                _connectivityChecker.Disabled = !hasInternet;
                OnConnectivityChanged?.Invoke(hasInternet);
                promise.CompleteSuccess();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error in SetHasInternet: " + ex.Message);
                promise.CompleteError(ex);
            }
            return promise;
        }

        public Promise ReportInternetLoss()
        {
            var promise = new Promise();
            try
            {
                Debug.LogError("Internet connection lost");
                _connectivityChecker.Disabled = true;
                OnConnectivityChanged?.Invoke(false);
                promise.CompleteSuccess();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error in ReportInternetLoss: " + ex.Message);
                promise.CompleteError(ex);
            }
            return promise;
        }

        public void OnReconnectOnce(Action onReconnection)
        {
            try
            {
                if (HasConnectivity) onReconnection.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error in OnReconnectOnce: " + ex.Message);
            }
        }

        // Global log handler to capture exceptions
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                Debug.LogWarning("Captured Exception Log: " + logString + "\n" + stackTrace);
            }
        }
    }
}
