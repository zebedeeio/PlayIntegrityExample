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
    
    
    /// <summary> A generated client for <see cref="Beamable.Microservices.GameServer"/> </summary
    public sealed class GameServerClient : MicroserviceClient, Beamable.Common.IHaveServiceName
    {
        
        public GameServerClient(BeamContext context = null) : 
                base(context)
        {
        }
        
        public string ServiceName
        {
            get
            {
                return "GameServer";
            }
        }
        
        /// <summary>
        /// Call the GetStats method on the GameServer microservice
        /// <see cref="Beamable.Microservices.GameServer.GetStats"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetStats()
        {
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            return this.Request<string>("GameServer", "GetStats", serializedFields);
        }
        
        /// <summary>
        /// Call the SendPlaytime method on the GameServer microservice
        /// <see cref="Beamable.Microservices.GameServer.SendPlaytime"/>
        /// </summary>
        public Beamable.Common.Promise<string> SendPlaytime(string userId)
        {
            object raw_userId = userId;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("userId", raw_userId);
            return this.Request<string>("GameServer", "SendPlaytime", serializedFields);
        }
        
        /// <summary>
        /// Call the WithdrawBitcoin method on the GameServer microservice
        /// <see cref="Beamable.Microservices.GameServer.WithdrawBitcoin"/>
        /// </summary>
        public Beamable.Common.Promise<string> WithdrawBitcoin(string username)
        {
            object raw_username = username;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("username", raw_username);
            return this.Request<string>("GameServer", "WithdrawBitcoin", serializedFields);
        }
    }
    
    internal sealed class MicroserviceParametersGameServerClient
    {
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_String : MicroserviceClientDataWrapper<string>
        {
        }
    }
    
    [BeamContextSystemAttribute()]
    public static class ExtensionsForGameServerClient
    {
        
        [Beamable.Common.Dependencies.RegisterBeamableDependenciesAttribute()]
        public static void RegisterService(Beamable.Common.Dependencies.IDependencyBuilder builder)
        {
            builder.AddScoped<GameServerClient>();
        }
        
        public static GameServerClient GameServer(this Beamable.Server.MicroserviceClients clients)
        {
            return clients.GetClient<GameServerClient>();
        }
    }
}
