using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MessageFramework.DataObjects;
using System.ServiceModel.Channels;

namespace GUIFramework
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [ServiceContractAttribute(ConfigurationName = "IMessage", CallbackContract = typeof(IMessageCallback), SessionMode = SessionMode.Required)]
    public interface IMessage
    {

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendPropertyMessage")]
        void SendPropertyMessage(APIPropertyMessage property);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendPropertyMessage")]
        Task SendPropertyMessageAsync(APIPropertyMessage property);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendListMessage")]
        void SendListMessage(APIListMessage listData);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendListMessage")]
        Task SendListMessageAsync(APIListMessage listData);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendInfoMessage")]
        void SendInfoMessage(APIInfoMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendInfoMessage")]
        Task SendInfoMessageAsync(APIInfoMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendDataMessage")]
        void SendDataMessage(APIDataMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendDataMessage")]
        Task SendDataMessageAsync(APIDataMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendMediaPortalMessage")]
        void SendMediaPortalMessage(APIMediaPortalMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendMediaPortalMessage")]
        Task SendMediaPortalMessageAsync(APIMediaPortalMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendTVServerMessage")]
        void SendTVServerMessage(APITVServerMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendTVServerMessage")]
        Task SendTVServerMessageAsync(APITVServerMessage msg);

        [OperationContractAttribute(Action = "http://tempuri.org/IMessage/Connect", ReplyAction = "http://tempuri.org/IMessage/ConnectResponse")]
        List<APIConnection> Connect(APIConnection name);

        [OperationContractAttribute(Action = "http://tempuri.org/IMessage/Connect", ReplyAction = "http://tempuri.org/IMessage/ConnectResponse")]
        Task<List<APIConnection>> ConnectAsync(APIConnection name);

        [OperationContractAttribute(IsOneWay = true, IsTerminating = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/Disconnect")]
        void Disconnect();

        [OperationContractAttribute(IsOneWay = true, IsTerminating = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/Disconnect")]
        Task DisconnectAsync();
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMessageCallback
    {

        [OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveAPIPropertyMessage")]
        void ReceiveAPIPropertyMessage(APIPropertyMessage message);

        [OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveAPIListMessage")]
        void ReceiveAPIListMessage(APIListMessage message);

        [OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveAPIInfoMessage")]
        void ReceiveAPIInfoMessage(APIInfoMessage message);

        [OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveAPIDataMessage")]
        void ReceiveAPIDataMessage(APIDataMessage message);

        [OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveMediaPortalMessage")]
        void ReceiveMediaPortalMessage(APIMediaPortalMessage message);

        [OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IMessage/ReceiveTVServerMessage")]
        void ReceiveTVServerMessage(APITVServerMessage message);

        [OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IMessage/SessionConnected")]
        void SessionConnected(APIConnection connection);

        [OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IMessage/SessionDisconnected")]
        void SessionDisconnected(APIConnection connection);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMessageChannel : IMessage, IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MessageClient : DuplexClientBase<IMessage>, IMessage
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
            base.Channel.SendPropertyMessage(property);
        }

        public Task SendPropertyMessageAsync(APIPropertyMessage property)
        {
            return base.Channel.SendPropertyMessageAsync(property);
        }

        public void SendListMessage(APIListMessage listData)
        {
            base.Channel.SendListMessage(listData);
        }

        public Task SendListMessageAsync(APIListMessage listData)
        {
            return base.Channel.SendListMessageAsync(listData);
        }

        public void SendInfoMessage(APIInfoMessage msg)
        {
            base.Channel.SendInfoMessage(msg);
        }

        public Task SendInfoMessageAsync(APIInfoMessage msg)
        {
            return base.Channel.SendInfoMessageAsync(msg);
        }

        public void SendDataMessage(APIDataMessage msg)
        {
            base.Channel.SendDataMessage(msg);
        }

        public Task SendDataMessageAsync(APIDataMessage msg)
        {
            return base.Channel.SendDataMessageAsync(msg);
        }

        public void SendMediaPortalMessage(APIMediaPortalMessage msg)
        {
            base.Channel.SendMediaPortalMessage(msg);
        }

        public Task SendMediaPortalMessageAsync(APIMediaPortalMessage msg)
        {
            return base.Channel.SendMediaPortalMessageAsync(msg);
        }

        public void SendTVServerMessage(APITVServerMessage msg)
        {
            base.Channel.SendTVServerMessage(msg);
        }

        public Task SendTVServerMessageAsync(APITVServerMessage msg)
        {
            return base.Channel.SendTVServerMessageAsync(msg);
        }

        public List<APIConnection> Connect(APIConnection name)
        {
            return base.Channel.Connect(name);
        }

        public Task<List<APIConnection>> ConnectAsync(APIConnection name)
        {
            return base.Channel.ConnectAsync(name);
        }

        public void Disconnect()
        {
            base.Channel.Disconnect();
        }

        public Task DisconnectAsync()
        {
            return base.Channel.DisconnectAsync();
        }
    }
}
