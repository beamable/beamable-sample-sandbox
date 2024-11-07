using System;
using Beamable.Api.Connectivity;
using Beamable.Common;

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
        }

        public void OnReconnectOnce(ConnectionCallback promise, int order = 0)
        {
            throw new NotImplementedException();
        }

        public bool HasConnectivity => _connectivityChecker.HasConnectivity;
        public bool ForceDisabled
        {
            get => _connectivityChecker.Disabled;
            set => _connectivityChecker.Disabled = value;
        }

        private void ConnectivityChangedHandler(bool hasConnectivity)
        {
            OnConnectivityChanged?.Invoke(hasConnectivity);
        }

        public Promise SetHasInternet(bool hasInternet)
        {
            var promise = new Promise();
            _connectivityChecker.Disabled = !hasInternet;
            promise.CompleteSuccess();
            return promise;
        }

        public Promise ReportInternetLoss()
        {
            throw new NotImplementedException();
        }

        public void OnReconnectOnce(Action onReconnection)
        {
            throw new NotImplementedException();
        }
        

    }
}