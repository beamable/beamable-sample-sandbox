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
        /// Call the CreateLobby method on the Service microservice
        /// <see cref="Beamable.Microservices.Service.CreateLobby"/>
        /// </summary>
        public Beamable.Common.Promise<Beamable.Common.Utils.Response<Beamable.Common.Models.LobbyData>> CreateLobby(string lobbyName, long hostId)
        {
            object raw_lobbyName = lobbyName;
            object raw_hostId = hostId;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("lobbyName", raw_lobbyName);
            serializedFields.Add("hostId", raw_hostId);
            return this.Request<Beamable.Common.Utils.Response<Beamable.Common.Models.LobbyData>>("Service", "CreateLobby", serializedFields);
        }
        
        /// <summary>
        /// Call the GetLobby method on the Service microservice
        /// <see cref="Beamable.Microservices.Service.GetLobby"/>
        /// </summary>
        public Beamable.Common.Promise<Beamable.Common.Utils.Response<Beamable.Common.Models.LobbyData>> GetLobby(string lobbyName)
        {
            object raw_lobbyName = lobbyName;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("lobbyName", raw_lobbyName);
            return this.Request<Beamable.Common.Utils.Response<Beamable.Common.Models.LobbyData>>("Service", "GetLobby", serializedFields);
        }
        
        /// <summary>
        /// Call the AddMemberToLobby method on the Service microservice
        /// <see cref="Beamable.Microservices.Service.AddMemberToLobby"/>
        /// </summary>
        public Beamable.Common.Promise<Beamable.Common.Utils.Response<bool>> AddMemberToLobby(string lobbyName, long memberId)
        {
            object raw_lobbyName = lobbyName;
            object raw_memberId = memberId;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("lobbyName", raw_lobbyName);
            serializedFields.Add("memberId", raw_memberId);
            return this.Request<Beamable.Common.Utils.Response<bool>>("Service", "AddMemberToLobby", serializedFields);
        }
        
        /// <summary>
        /// Call the SetHost method on the Service microservice
        /// <see cref="Beamable.Microservices.Service.SetHost"/>
        /// </summary>
        public Beamable.Common.Promise<Beamable.Common.Utils.Response<bool>> SetHost(string lobbyName, long newHostId)
        {
            object raw_lobbyName = lobbyName;
            object raw_newHostId = newHostId;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("lobbyName", raw_lobbyName);
            serializedFields.Add("newHostId", raw_newHostId);
            return this.Request<Beamable.Common.Utils.Response<bool>>("Service", "SetHost", serializedFields);
        }
    }
    
    internal sealed class MicroserviceParametersServiceClient
    {
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_String : MicroserviceClientDataWrapper<string>
        {
        }
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_Int64 : MicroserviceClientDataWrapper<long>
        {
        }
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
