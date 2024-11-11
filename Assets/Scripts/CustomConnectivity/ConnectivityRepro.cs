using System.Threading.Tasks;
using Beamable;
using Beamable.Api.Connectivity;
using DefaultNamespace;
using UnityEngine;

public class ConnectivityRepro : MonoBehaviour
{
    // Events to update the UI on connectivity changes
    [HideInInspector]
    public RefreshedUnityEvent onRefreshed = new RefreshedUnityEvent();

    private BeamContext _beamContext;
    private ConnectivityServiceData _data = new ConnectivityServiceData();
    private IConnectivityService _connectivityService;

    protected async void Start()
    {
        Debug.Log("Start() - Checking Internet Connection...");
        SetupConnectivityService(new CustomConnectivityService());
        await SetupBeamable();
    }

    private void SetupConnectivityService(IConnectivityService connectivityService) =>
        _connectivityService = connectivityService;

    private async Task SetupBeamable()
    {
        _beamContext = await BeamContext.Default.Instance;
        Debug.Log($"BeamContext PlayerId: {_beamContext.PlayerId}");

        _connectivityService.OnConnectivityChanged += ConnectivityService_OnConnectivityChanged;
    }

    public void ToggleHasInternet()
    {
        _connectivityService.SetHasInternet(!_data.HasConnectivity);
    }

    private void Refresh()
    {
        var refreshLog = $"Refresh()..." +
                            $"\n * HasConnectivity = {_data.HasConnectivity}";
        
        Debug.Log(refreshLog);

        onRefreshed?.Invoke(_data);
    }

    private void ConnectivityService_OnConnectivityChanged(bool hasConnectivity)
    {
        _data.HasConnectivity = hasConnectivity;
        Refresh();
    }
}
