using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using MessageFramework.DataObjects;
using MessageFramework.Messages;

namespace GUIFramework
{
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

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendTVServerMessage")]
        void SendTVServerMessage(APITVServerMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendTVServerMessage")]
        Task SendTVServerMessageAsync(APITVServerMessage msg);

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

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveTVServerMessage")]
        void ReceiveTVServerMessage(APITVServerMessage message);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IMessage/SessionConnected")]
        void SessionConnected(APIConnection connection);

        [OperationContract(IsOneWay = true, Action = "http://tempuri.org/IMessage/SessionDisconnected")]
        void SessionDisconnected(APIConnection connection);
    }

    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface IMessageChannel : IMessage, IClientChannel
    {
    }

    [DebuggerStepThrough()]
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

        public void SendTVServerMessage(APITVServerMessage msg)
        {
            Channel.SendTVServerMessage(msg);
        }

        public Task SendTVServerMessageAsync(APITVServerMessage msg)
        {
            return Channel.SendTVServerMessageAsync(msg);
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
