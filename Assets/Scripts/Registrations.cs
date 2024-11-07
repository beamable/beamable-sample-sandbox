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
        builder.RemoveIfExists<IConnectivityChecker>();
            
        builder.AddSingleton<IConnectivityChecker>(_ => new CustomConnectivityChecker());
            
        builder.RemoveIfExists<IConnectivityService>();
        builder.AddSingleton<IConnectivityService, CustomConnectivityService>();
    }
}