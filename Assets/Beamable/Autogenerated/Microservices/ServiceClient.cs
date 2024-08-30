//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Beamable.Server.Clients
{
    using System;
    using Beamable.Platform.SDK;
    using Beamable.Server;
    
    
    /// <summary> A generated client for <see cref="Beamable.Microservices.Service"/> </summary
    public sealed class ServiceClient : MicroserviceClient, Beamable.Common.IHaveServiceName
    {
        
        public ServiceClient(BeamContext context = null) : 
                base(context)
        {
        }
        
        public string ServiceName
        {
            get
            {
                return "Service";
            }
        }
        
        /// <summary>
        /// Call the Test method on the Service microservice
        /// <see cref="Beamable.Microservices.Service.Test"/>
        /// </summary>
        public Beamable.Common.Promise<Beamable.Common.Unit> Test()
        {
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            return this.Request<Beamable.Common.Unit>("Service", "Test", serializedFields);
        }
    }
    
    internal sealed class MicroserviceParametersServiceClient
    {
    }
    
    [BeamContextSystemAttribute()]
    public static class ExtensionsForServiceClient
    {
        
        [Beamable.Common.Dependencies.RegisterBeamableDependenciesAttribute()]
        public static void RegisterService(Beamable.Common.Dependencies.IDependencyBuilder builder)
        {
            builder.AddScoped<ServiceClient>();
        }
        
        public static ServiceClient Service(this Beamable.Server.MicroserviceClients clients)
        {
            return clients.GetClient<ServiceClient>();
        }
    }
}
