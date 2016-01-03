using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using MessageFramework.DataObjects;
using MessageFramework.Messages;

namespace MediaPortalPlugin
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [ServiceContract(ConfigurationName = "IMessage", CallbackContract = typeof(IMessageCallback), SessionMode = SessionMode.Required)]
    public interface IMessage
    {
        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendPropertyMessage")]
        void SendPropertyMessage(APIPropertyMessage property);

        [OperationContract(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendPropertyMessage")]
        // ReSharper disable once OneWayOperationContractWithReturnType
        IAsyncResult BeginSendPropertyMessage(APIPropertyMessage property, AsyncCallback callback, object asyncState);

        void EndSendPropertyMessage(IAsyncResult result);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendListMessage")]
        void SendListMessage(APIListMessage listData);

        [OperationContract(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendListMessage")]
        // ReSharper disable once OneWayOperationContractWithReturnType
        IAsyncResult BeginSendListMessage(APIListMessage listData, AsyncCallback callback, object asyncState);

        void EndSendListMessage(IAsyncResult result);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendInfoMessage")]
        void SendInfoMessage(APIInfoMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendInfoMessage")]
        // ReSharper disable once OneWayOperationContractWithReturnType
        IAsyncResult BeginSendInfoMessage(APIInfoMessage msg, AsyncCallback callback, object asyncState);

        void EndSendInfoMessage(IAsyncResult result);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendDataMessage")]
        void SendDataMessage(APIDataMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendDataMessage")]
        // ReSharper disable once OneWayOperationContractWithReturnType
        IAsyncResult BeginSendDataMessage(APIDataMessage msg, AsyncCallback callback, object asyncState);

        void EndSendDataMessage(IAsyncResult result);

        [OperationContract(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendMediaPortalMessage")]
        void SendMediaPortalMessage(APIMediaPortalMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendMediaPortalMessage")]
        // ReSharper disable once OneWayOperationContractWithReturnType
        IAsyncResult BeginSendMediaPortalMessage(APIMediaPortalMessage msg, AsyncCallback callback, object asyncState);

        void EndSendMediaPortalMessage(IAsyncResult result);

        [OperationContract(Action = "http://tempuri.org/IMessage/Connect", ReplyAction = "http://tempuri.org/IMessage/ConnectResponse")]
        List<APIConnection> Connect(APIConnection name);

        [OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IMessage/Connect", ReplyAction = "http://tempuri.org/IMessage/ConnectResponse")]
        IAsyncResult BeginConnect(APIConnection name, AsyncCallback callback, object asyncState);

        List<APIConnection> EndConnect(IAsyncResult result);

        [OperationContract(IsOneWay = true, IsTerminating = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/Disconnect")]
        void Disconnect();

        [OperationContract(IsOneWay = true, IsTerminating = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/Disconnect")]
        // ReSharper disable once OneWayOperationContractWithReturnType
        IAsyncResult BeginDisconnect(AsyncCallback callback, object asyncState);

        void EndDisconnect(IAsyncResult result);
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
    public class ConnectCompletedEventArgs : AsyncCompletedEventArgs
    {

        private readonly object[] _results;

        public ConnectCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            _results = results;
        }

        public List<APIConnection> Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (List<APIConnection>)_results[0];
            }
        }
    }

    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public class MessageClient : DuplexClientBase<IMessage>, IMessage
    {

        private BeginOperationDelegate _onBeginSendPropertyMessageDelegate;

        private EndOperationDelegate _onEndSendPropertyMessageDelegate;

        private SendOrPostCallback _onSendPropertyMessageCompletedDelegate;

        private BeginOperationDelegate _onBeginSendListMessageDelegate;

        private EndOperationDelegate _onEndSendListMessageDelegate;

        private SendOrPostCallback _onSendListMessageCompletedDelegate;

        private BeginOperationDelegate _onBeginSendInfoMessageDelegate;

        private EndOperationDelegate _onEndSendInfoMessageDelegate;

        private SendOrPostCallback _onSendInfoMessageCompletedDelegate;

        private BeginOperationDelegate _onBeginSendDataMessageDelegate;

        private EndOperationDelegate _onEndSendDataMessageDelegate;

        private SendOrPostCallback _onSendDataMessageCompletedDelegate;

        private BeginOperationDelegate _onBeginSendMediaPortalMessageDelegate;

        private EndOperationDelegate _onEndSendMediaPortalMessageDelegate;

        private SendOrPostCallback _onSendMediaPortalMessageCompletedDelegate;

        private BeginOperationDelegate _onBeginConnectDelegate;

        private EndOperationDelegate _onEndConnectDelegate;

        private SendOrPostCallback _onConnectCompletedDelegate;

        private BeginOperationDelegate _onBeginDisconnectDelegate;

        private EndOperationDelegate _onEndDisconnectDelegate;

        private SendOrPostCallback _onDisconnectCompletedDelegate;

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

        public event EventHandler<AsyncCompletedEventArgs> SendPropertyMessageCompleted;

        public event EventHandler<AsyncCompletedEventArgs> SendListMessageCompleted;

        public event EventHandler<AsyncCompletedEventArgs> SendInfoMessageCompleted;

        public event EventHandler<AsyncCompletedEventArgs> SendDataMessageCompleted;

        public event EventHandler<AsyncCompletedEventArgs> SendMediaPortalMessageCompleted;

        public event EventHandler<ConnectCompletedEventArgs> ConnectCompleted;

        public event EventHandler<AsyncCompletedEventArgs> DisconnectCompleted;

        public void SendPropertyMessage(APIPropertyMessage property)
        {
            Channel.SendPropertyMessage(property);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendPropertyMessage(APIPropertyMessage property, AsyncCallback callback, object asyncState)
        {
            return Channel.BeginSendPropertyMessage(property, callback, asyncState);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void EndSendPropertyMessage(IAsyncResult result)
        {
            Channel.EndSendPropertyMessage(result);
        }

        private IAsyncResult OnBeginSendPropertyMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            var property = (APIPropertyMessage)inValues[0];
            return BeginSendPropertyMessage(property, callback, asyncState);
        }

        private object[] OnEndSendPropertyMessage(IAsyncResult result)
        {
            EndSendPropertyMessage(result);
            return null;
        }

        private void OnSendPropertyMessageCompleted(object state)
        {
            if (SendPropertyMessageCompleted == null) return;

            var e = (InvokeAsyncCompletedEventArgs)state;
            SendPropertyMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }

        public void SendPropertyMessageAsync(APIPropertyMessage property)
        {
            SendPropertyMessageAsync(property, null);
        }

        public void SendPropertyMessageAsync(APIPropertyMessage property, object userState)
        {
            if (_onBeginSendPropertyMessageDelegate == null)
            {
                _onBeginSendPropertyMessageDelegate = OnBeginSendPropertyMessage;
            }
            if (_onEndSendPropertyMessageDelegate == null)
            {
                _onEndSendPropertyMessageDelegate = OnEndSendPropertyMessage;
            }
            if (_onSendPropertyMessageCompletedDelegate == null)
            {
                _onSendPropertyMessageCompletedDelegate = OnSendPropertyMessageCompleted;
            }
            InvokeAsync(_onBeginSendPropertyMessageDelegate, new object[] {
                    property}, _onEndSendPropertyMessageDelegate, _onSendPropertyMessageCompletedDelegate, userState);
        }

        public void SendListMessage(APIListMessage listData)
        {
            Channel.SendListMessage(listData);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendListMessage(APIListMessage listData, AsyncCallback callback, object asyncState)
        {
            return Channel.BeginSendListMessage(listData, callback, asyncState);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void EndSendListMessage(IAsyncResult result)
        {
            Channel.EndSendListMessage(result);
        }

        private IAsyncResult OnBeginSendListMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            var listData = (APIListMessage)inValues[0];
            return BeginSendListMessage(listData, callback, asyncState);
        }

        private object[] OnEndSendListMessage(IAsyncResult result)
        {
            EndSendListMessage(result);
            return null;
        }

        private void OnSendListMessageCompleted(object state)
        {
            if (SendListMessageCompleted == null) return;

            var e = (InvokeAsyncCompletedEventArgs)state;
            SendListMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }

        public void SendListMessageAsync(APIListMessage listData)
        {
            SendListMessageAsync(listData, null);
        }

        public void SendListMessageAsync(APIListMessage listData, object userState)
        {
            if (_onBeginSendListMessageDelegate == null)
            {
                _onBeginSendListMessageDelegate = OnBeginSendListMessage;
            }
            if (_onEndSendListMessageDelegate == null)
            {
                _onEndSendListMessageDelegate = OnEndSendListMessage;
            }
            if (_onSendListMessageCompletedDelegate == null)
            {
                _onSendListMessageCompletedDelegate = OnSendListMessageCompleted;
            }
            InvokeAsync(_onBeginSendListMessageDelegate, new object[] {
                    listData}, _onEndSendListMessageDelegate, _onSendListMessageCompletedDelegate, userState);
        }

        public void SendInfoMessage(APIInfoMessage msg)
        {
            Channel.SendInfoMessage(msg);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendInfoMessage(APIInfoMessage msg, AsyncCallback callback, object asyncState)
        {
            return Channel.BeginSendInfoMessage(msg, callback, asyncState);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void EndSendInfoMessage(IAsyncResult result)
        {
            Channel.EndSendInfoMessage(result);
        }

        private IAsyncResult OnBeginSendInfoMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            var msg = (APIInfoMessage)inValues[0];
            return BeginSendInfoMessage(msg, callback, asyncState);
        }

        private object[] OnEndSendInfoMessage(IAsyncResult result)
        {
            EndSendInfoMessage(result);
            return null;
        }

        private void OnSendInfoMessageCompleted(object state)
        {
            if (SendInfoMessageCompleted == null) return;

            var e = (InvokeAsyncCompletedEventArgs)state;
            SendInfoMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }

        public void SendInfoMessageAsync(APIInfoMessage msg)
        {
            SendInfoMessageAsync(msg, null);
        }

        public void SendInfoMessageAsync(APIInfoMessage msg, object userState)
        {
            if (_onBeginSendInfoMessageDelegate == null)
            {
                _onBeginSendInfoMessageDelegate = OnBeginSendInfoMessage;
            }
            if (_onEndSendInfoMessageDelegate == null)
            {
                _onEndSendInfoMessageDelegate = OnEndSendInfoMessage;
            }
            if (_onSendInfoMessageCompletedDelegate == null)
            {
                _onSendInfoMessageCompletedDelegate = OnSendInfoMessageCompleted;
            }
            InvokeAsync(_onBeginSendInfoMessageDelegate, new object[] {
                    msg}, _onEndSendInfoMessageDelegate, _onSendInfoMessageCompletedDelegate, userState);
        }

        public void SendDataMessage(APIDataMessage msg)
        {
            Channel.SendDataMessage(msg);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendDataMessage(APIDataMessage msg, AsyncCallback callback, object asyncState)
        {
            return Channel.BeginSendDataMessage(msg, callback, asyncState);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void EndSendDataMessage(IAsyncResult result)
        {
            Channel.EndSendDataMessage(result);
        }

        private IAsyncResult OnBeginSendDataMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            var msg = (APIDataMessage)inValues[0];
            return BeginSendDataMessage(msg, callback, asyncState);
        }

        private object[] OnEndSendDataMessage(IAsyncResult result)
        {
            EndSendDataMessage(result);
            return null;
        }

        private void OnSendDataMessageCompleted(object state)
        {
            if (SendDataMessageCompleted == null) return;

            var e = (InvokeAsyncCompletedEventArgs)state;
            SendDataMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }

        public void SendDataMessageAsync(APIDataMessage msg)
        {
            SendDataMessageAsync(msg, null);
        }

        public void SendDataMessageAsync(APIDataMessage msg, object userState)
        {
            if (_onBeginSendDataMessageDelegate == null)
            {
                _onBeginSendDataMessageDelegate = OnBeginSendDataMessage;
            }
            if (_onEndSendDataMessageDelegate == null)
            {
                _onEndSendDataMessageDelegate = OnEndSendDataMessage;
            }
            if (_onSendDataMessageCompletedDelegate == null)
            {
                _onSendDataMessageCompletedDelegate = OnSendDataMessageCompleted;
            }
            InvokeAsync(_onBeginSendDataMessageDelegate, new object[] {
                    msg}, _onEndSendDataMessageDelegate, _onSendDataMessageCompletedDelegate, userState);
        }

        public void SendMediaPortalMessage(APIMediaPortalMessage msg)
        {
            Channel.SendMediaPortalMessage(msg);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendMediaPortalMessage(APIMediaPortalMessage msg, AsyncCallback callback, object asyncState)
        {
            return Channel.BeginSendMediaPortalMessage(msg, callback, asyncState);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void EndSendMediaPortalMessage(IAsyncResult result)
        {
            Channel.EndSendMediaPortalMessage(result);
        }

        private IAsyncResult OnBeginSendMediaPortalMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            var msg = (APIMediaPortalMessage)inValues[0];
            return BeginSendMediaPortalMessage(msg, callback, asyncState);
        }

        private object[] OnEndSendMediaPortalMessage(IAsyncResult result)
        {
            EndSendMediaPortalMessage(result);
            return null;
        }

        private void OnSendMediaPortalMessageCompleted(object state)
        {
            if (SendMediaPortalMessageCompleted == null) return;

            var e = (InvokeAsyncCompletedEventArgs)state;
            SendMediaPortalMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }

        public void SendMediaPortalMessageAsync(APIMediaPortalMessage msg)
        {
            SendMediaPortalMessageAsync(msg, null);
        }

        public void SendMediaPortalMessageAsync(APIMediaPortalMessage msg, object userState)
        {
            if (_onBeginSendMediaPortalMessageDelegate == null)
            {
                _onBeginSendMediaPortalMessageDelegate = OnBeginSendMediaPortalMessage;
            }
            if (_onEndSendMediaPortalMessageDelegate == null)
            {
                _onEndSendMediaPortalMessageDelegate = OnEndSendMediaPortalMessage;
            }
            if (_onSendMediaPortalMessageCompletedDelegate == null)
            {
                _onSendMediaPortalMessageCompletedDelegate = OnSendMediaPortalMessageCompleted;
            }
            InvokeAsync(_onBeginSendMediaPortalMessageDelegate, new object[] {
                    msg}, _onEndSendMediaPortalMessageDelegate, _onSendMediaPortalMessageCompletedDelegate, userState);
        }


        public List<APIConnection> Connect(APIConnection name)
        {
            return Channel.Connect(name);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginConnect(APIConnection name, AsyncCallback callback, object asyncState)
        {
            return Channel.BeginConnect(name, callback, asyncState);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public List<APIConnection> EndConnect(IAsyncResult result)
        {
            return Channel.EndConnect(result);
        }

        private IAsyncResult OnBeginConnect(object[] inValues, AsyncCallback callback, object asyncState)
        {
            var name = (APIConnection)inValues[0];
            return BeginConnect(name, callback, asyncState);
        }

        private object[] OnEndConnect(IAsyncResult result)
        {
            var retVal = EndConnect(result);
            return new object[] {
                retVal};
        }

        private void OnConnectCompleted(object state)
        {
            if (ConnectCompleted == null) return;

            var e = (InvokeAsyncCompletedEventArgs)state;
            ConnectCompleted(this, new ConnectCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
        }

        public void ConnectAsync(APIConnection name)
        {
            ConnectAsync(name, null);
        }

        public void ConnectAsync(APIConnection name, object userState)
        {
            if (_onBeginConnectDelegate == null)
            {
                _onBeginConnectDelegate = OnBeginConnect;
            }
            if (_onEndConnectDelegate == null)
            {
                _onEndConnectDelegate = OnEndConnect;
            }
            if (_onConnectCompletedDelegate == null)
            {
                _onConnectCompletedDelegate = OnConnectCompleted;
            }
            InvokeAsync(_onBeginConnectDelegate, new object[] {
                    name}, _onEndConnectDelegate, _onConnectCompletedDelegate, userState);
        }

        public void Disconnect()
        {
            Channel.Disconnect();
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginDisconnect(AsyncCallback callback, object asyncState)
        {
            return Channel.BeginDisconnect(callback, asyncState);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void EndDisconnect(IAsyncResult result)
        {
            Channel.EndDisconnect(result);
        }

        private IAsyncResult OnBeginDisconnect(object[] inValues, AsyncCallback callback, object asyncState)
        {
            return BeginDisconnect(callback, asyncState);
        }

        private object[] OnEndDisconnect(IAsyncResult result)
        {
            EndDisconnect(result);
            return null;
        }

        private void OnDisconnectCompleted(object state)
        {
            if (DisconnectCompleted == null) return;

            var e = (InvokeAsyncCompletedEventArgs)state;
            DisconnectCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }

        public void DisconnectAsync()
        {
            DisconnectAsync(null);
        }

        public void DisconnectAsync(object userState)
        {
            if (_onBeginDisconnectDelegate == null)
            {
                _onBeginDisconnectDelegate = OnBeginDisconnect;
            }
            if (_onEndDisconnectDelegate == null)
            {
                _onEndDisconnectDelegate = OnEndDisconnect;
            }
            if (_onDisconnectCompletedDelegate == null)
            {
                _onDisconnectCompletedDelegate = OnDisconnectCompleted;
            }
            InvokeAsync(_onBeginDisconnectDelegate, null, _onEndDisconnectDelegate, _onDisconnectCompletedDelegate, userState);
        }
    }
}