using Beamable;
using Beamable.Api.Connectivity;
using Beamable.Common.Api;
using Beamable.Common.Dependencies;
using DefaultNamespace;

[BeamContextSystem]
public class Registrations
{
    [RegisterBeamableDependencies]
    public static void Build(IDependencyBuilder builder)
    {
        // Ensure existing services are removed if they were registered
        builder.RemoveIfExists<IConnectivityChecker>();
            
        // Register CustomConnectivityChecker with a default interval
        builder.AddSingleton<IConnectivityChecker>(_ => new CustomConnectivityChecker());
            
        // Register CustomConnectivityService as IConnectivityService
        builder.RemoveIfExists<IConnectivityService>();
        builder.AddSingleton<IConnectivityService, CustomConnectivityService>();
    }
}