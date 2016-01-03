using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Windows;
using Common.Helpers;
using Common.Settings;
using MessageFramework.DataObjects;
using MessageFramework.Messages;

namespace GUIConfig.Settings
{

        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ConnectionTester : IMessageCallback
    {
    
        #region Singleton Implementation

        private ConnectionTester() { }
        private static ConnectionTester _instance;
        public static ConnectionTester Instance => _instance ?? (_instance = new ConnectionTester());

            public static Task<bool> TestConnection(ConnectionSettings settings)
        {
           return Instance.TestServerConnection(settings);
        }

             #endregion

        public async Task<bool> TestServerConnection(ConnectionSettings settings)
        {
            try
            {
               
                var connectionString = $"net.tcp://{settings.IpAddress}:{settings.Port}/MPDisplayService";
                var serverEndpoint = new EndpointAddress(connectionString);
                var serverBinding = ConnectHelper.GetServerBinding();
                var site = new InstanceContext(this);
                var messageClient = new MessageClient(site, serverBinding, serverEndpoint);
                var connection = new APIConnection(ConnectionType.MediaPortalPlugin);

                var connections = await messageClient.ConnectAsync(connection);
                return connections != null && connections.Any();
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "Failed to connect to MPDisplay server");
            }
            return false;
        }

        public void ReceiveMediaPortalMessage(APIMediaPortalMessage message) { }
        public void ReceiveAPIPropertyMessage(APIPropertyMessage message) { }
        public void ReceiveAPIListMessage(APIListMessage message) { }
        public void ReceiveAPIInfoMessage(APIInfoMessage message) { }
        public void ReceiveAPIDataMessage(APIDataMessage message) { }
        public void SessionConnected(APIConnection connection) { }
        public void SessionDisconnected(APIConnection connection) { }
    }





    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [ServiceContract(ConfigurationName = "IMessage", CallbackContract = typeof(IMessageCallback), SessionMode = SessionMode.Required)]
    public interface IMessage
    {

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendPropertyMessage")]
        void SendPropertyMessage(APIPropertyMessage property);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendPropertyMessage")]
        Task SendPropertyMessageAsync(APIPropertyMessage property);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendListMessage")]
        void SendListMessage(APIListMessage listData);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendListMessage")]
        Task SendListMessageAsync(APIListMessage listData);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendInfoMessage")]
        void SendInfoMessage(APIInfoMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendInfoMessage")]
        Task SendInfoMessageAsync(APIInfoMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendDataMessage")]
        void SendDataMessage(APIDataMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendDataMessage")]
        Task SendDataMessageAsync(APIDataMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendMediaPortalMessage")]
        void SendMediaPortalMessage(APIMediaPortalMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendMediaPortalMessage")]
        Task SendMediaPortalMessageAsync(APIMediaPortalMessage msg);

        [OperationContract(Action = "http://tempuri.org/IMessage/Connect", ReplyAction = "http://tempuri.org/IMessage/ConnectResponse")]
        List<APIConnection> Connect(APIConnection name);

        [OperationContract(Action = "http://tempuri.org/IMessage/Connect", ReplyAction = "http://tempuri.org/IMessage/ConnectResponse")]
        Task<List<APIConnection>> ConnectAsync(APIConnection name);

        [OperationContract(IsOneWay = true, IsTerminating = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/Disconnect")]
        void Disconnect();

        [OperationContract(IsOneWay = true, IsTerminating = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/Disconnect")]
        Task DisconnectAsync();
    }

    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [ServiceContract]
    public interface IMessageCallback
    {

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveAPIPropertyMessage")]
        void ReceiveAPIPropertyMessage(APIPropertyMessage message);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveAPIListMessage")]
        void ReceiveAPIListMessage(APIListMessage message);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveAPIInfoMessage")]
        void ReceiveAPIInfoMessage(APIInfoMessage message);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveAPIDataMessage")]
        void ReceiveAPIDataMessage(APIDataMessage message);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveMediaPortalMessage")]
        void ReceiveMediaPortalMessage(APIMediaPortalMessage message);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IMessage/SessionConnected")]
        void SessionConnected(APIConnection connection);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IMessage/SessionDisconnected")]
        void SessionDisconnected(APIConnection connection);
    }

    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface IMessageChannel : IMessage, IClientChannel
    {
    }

    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public class MessageClient : DuplexClientBase<IMessage>, IMessage
    {

        public MessageClient(InstanceContext callbackInstance) :
            base(callbackInstance)
        {
        }

        public MessageClient(InstanceContext callbackInstance, string endpointConfigurationName) :
            base(callbackInstance, endpointConfigurationName)
        {
        }

        public MessageClient(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) :
            base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public MessageClient(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public MessageClient(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) :
            base(callbackInstance, binding, remoteAddress)
        {
        }

        public void SendPropertyMessage(APIPropertyMessage property)
        {
            Channel.SendPropertyMessage(property);
        }

        public Task SendPropertyMessageAsync(APIPropertyMessage property)
        {
            return Channel.SendPropertyMessageAsync(property);
        }

        public void SendListMessage(APIListMessage listData)
        {
            Channel.SendListMessage(listData);
        }

        public Task SendListMessageAsync(APIListMessage listData)
        {
            return Channel.SendListMessageAsync(listData);
        }

        public void SendInfoMessage(APIInfoMessage msg)
        {
            Channel.SendInfoMessage(msg);
        }

        public Task SendInfoMessageAsync(APIInfoMessage msg)
        {
            return Channel.SendInfoMessageAsync(msg);
        }

        public void SendDataMessage(APIDataMessage msg)
        {
            Channel.SendDataMessage(msg);
        }

        public Task SendDataMessageAsync(APIDataMessage msg)
        {
            return Channel.SendDataMessageAsync(msg);
        }

        public void SendMediaPortalMessage(APIMediaPortalMessage msg)
        {
            Channel.SendMediaPortalMessage(msg);
        }

        public Task SendMediaPortalMessageAsync(APIMediaPortalMessage msg)
        {
            return Channel.SendMediaPortalMessageAsync(msg);
        }

        public List<APIConnection> Connect(APIConnection name)
        {
            return Channel.Connect(name);
        }

        public Task<List<APIConnection>> ConnectAsync(APIConnection name)
        {
            return Channel.ConnectAsync(name);
        }

        public void Disconnect()
        {
            Channel.Disconnect();
        }

        public Task DisconnectAsync()
        {
            return Channel.DisconnectAsync();
        }
    }
}
