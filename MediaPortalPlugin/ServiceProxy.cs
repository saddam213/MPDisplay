using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [ServiceContractAttribute(ConfigurationName = "IMessage", CallbackContract = typeof(IMessageCallback), SessionMode = SessionMode.Required)]
    public interface IMessage
    {
        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendPropertyMessage")]
        void SendPropertyMessage(APIPropertyMessage property);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendPropertyMessage")]
        IAsyncResult BeginSendPropertyMessage(APIPropertyMessage property, AsyncCallback callback, object asyncState);

        void EndSendPropertyMessage(IAsyncResult result);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendListMessage")]
        void SendListMessage(APIListMessage listData);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendListMessage")]
        IAsyncResult BeginSendListMessage(APIListMessage listData, AsyncCallback callback, object asyncState);

        void EndSendListMessage(IAsyncResult result);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendInfoMessage")]
        void SendInfoMessage(APIInfoMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendInfoMessage")]
        IAsyncResult BeginSendInfoMessage(APIInfoMessage msg, AsyncCallback callback, object asyncState);

        void EndSendInfoMessage(IAsyncResult result);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendDataMessage")]
        void SendDataMessage(APIDataMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendDataMessage")]
        IAsyncResult BeginSendDataMessage(APIDataMessage msg, AsyncCallback callback, object asyncState);

        void EndSendDataMessage(IAsyncResult result);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendMediaPortalMessage")]
        void SendMediaPortalMessage(APIMediaPortalMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendMediaPortalMessage")]
        IAsyncResult BeginSendMediaPortalMessage(APIMediaPortalMessage msg, AsyncCallback callback, object asyncState);

        void EndSendMediaPortalMessage(IAsyncResult result);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/SendTVServerMessage")]
        void SendTVServerMessage(APITVServerMessage msg);

        [OperationContractAttribute(IsOneWay = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/SendTVServerMessage")]
        IAsyncResult BeginSendTVServerMessage(APITVServerMessage msg, AsyncCallback callback, object asyncState);

        void EndSendTVServerMessage(IAsyncResult result);

        [OperationContractAttribute(Action = "http://tempuri.org/IMessage/Connect", ReplyAction = "http://tempuri.org/IMessage/ConnectResponse")]
        List<APIConnection> Connect(APIConnection name);

        [OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IMessage/Connect", ReplyAction = "http://tempuri.org/IMessage/ConnectResponse")]
        IAsyncResult BeginConnect(APIConnection name, AsyncCallback callback, object asyncState);

        List<APIConnection> EndConnect(IAsyncResult result);

        [OperationContractAttribute(IsOneWay = true, IsTerminating = true, IsInitiating = false, Action = "http://tempuri.org/IMessage/Disconnect")]
        void Disconnect();

        [OperationContractAttribute(IsOneWay = true, IsTerminating = true, IsInitiating = false, AsyncPattern = true, Action = "http://tempuri.org/IMessage/Disconnect")]
        IAsyncResult BeginDisconnect(AsyncCallback callback, object asyncState);

        void EndDisconnect(IAsyncResult result);
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
    public partial class ConnectCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        public ConnectCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public List<APIConnection> Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return ((List<APIConnection>)(this.results[0]));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MessageClient : DuplexClientBase<IMessage>, IMessage
    {

        private BeginOperationDelegate onBeginSendPropertyMessageDelegate;

        private EndOperationDelegate onEndSendPropertyMessageDelegate;

        private SendOrPostCallback onSendPropertyMessageCompletedDelegate;

        private BeginOperationDelegate onBeginSendListMessageDelegate;

        private EndOperationDelegate onEndSendListMessageDelegate;

        private SendOrPostCallback onSendListMessageCompletedDelegate;

        private BeginOperationDelegate onBeginSendInfoMessageDelegate;

        private EndOperationDelegate onEndSendInfoMessageDelegate;

        private SendOrPostCallback onSendInfoMessageCompletedDelegate;

        private BeginOperationDelegate onBeginSendDataMessageDelegate;

        private EndOperationDelegate onEndSendDataMessageDelegate;

        private SendOrPostCallback onSendDataMessageCompletedDelegate;

        private BeginOperationDelegate onBeginSendMediaPortalMessageDelegate;

        private EndOperationDelegate onEndSendMediaPortalMessageDelegate;

        private SendOrPostCallback onSendMediaPortalMessageCompletedDelegate;

        private BeginOperationDelegate onBeginSendTVServerMessageDelegate;

        private EndOperationDelegate onEndSendTVServerMessageDelegate;

        private SendOrPostCallback onSendTVServerMessageCompletedDelegate;

        private BeginOperationDelegate onBeginConnectDelegate;

        private EndOperationDelegate onEndConnectDelegate;

        private SendOrPostCallback onConnectCompletedDelegate;

        private BeginOperationDelegate onBeginDisconnectDelegate;

        private EndOperationDelegate onEndDisconnectDelegate;

        private SendOrPostCallback onDisconnectCompletedDelegate;

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

        public event EventHandler<AsyncCompletedEventArgs> SendTVServerMessageCompleted;

        public event EventHandler<ConnectCompletedEventArgs> ConnectCompleted;

        public event EventHandler<AsyncCompletedEventArgs> DisconnectCompleted;

        public void SendPropertyMessage(APIPropertyMessage property)
        {
            base.Channel.SendPropertyMessage(property);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendPropertyMessage(APIPropertyMessage property, AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginSendPropertyMessage(property, callback, asyncState);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public void EndSendPropertyMessage(IAsyncResult result)
        {
            base.Channel.EndSendPropertyMessage(result);
        }

        private IAsyncResult OnBeginSendPropertyMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            APIPropertyMessage property = ((APIPropertyMessage)(inValues[0]));
            return this.BeginSendPropertyMessage(property, callback, asyncState);
        }

        private object[] OnEndSendPropertyMessage(IAsyncResult result)
        {
            this.EndSendPropertyMessage(result);
            return null;
        }

        private void OnSendPropertyMessageCompleted(object state)
        {
            if ((this.SendPropertyMessageCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SendPropertyMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void SendPropertyMessageAsync(APIPropertyMessage property)
        {
            this.SendPropertyMessageAsync(property, null);
        }

        public void SendPropertyMessageAsync(APIPropertyMessage property, object userState)
        {
            if ((this.onBeginSendPropertyMessageDelegate == null))
            {
                this.onBeginSendPropertyMessageDelegate = new BeginOperationDelegate(this.OnBeginSendPropertyMessage);
            }
            if ((this.onEndSendPropertyMessageDelegate == null))
            {
                this.onEndSendPropertyMessageDelegate = new EndOperationDelegate(this.OnEndSendPropertyMessage);
            }
            if ((this.onSendPropertyMessageCompletedDelegate == null))
            {
                this.onSendPropertyMessageCompletedDelegate = new SendOrPostCallback(this.OnSendPropertyMessageCompleted);
            }
            base.InvokeAsync(this.onBeginSendPropertyMessageDelegate, new object[] {
                    property}, this.onEndSendPropertyMessageDelegate, this.onSendPropertyMessageCompletedDelegate, userState);
        }

        public void SendListMessage(APIListMessage listData)
        {
            base.Channel.SendListMessage(listData);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendListMessage(APIListMessage listData, AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginSendListMessage(listData, callback, asyncState);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public void EndSendListMessage(IAsyncResult result)
        {
            base.Channel.EndSendListMessage(result);
        }

        private IAsyncResult OnBeginSendListMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            APIListMessage listData = ((APIListMessage)(inValues[0]));
            return this.BeginSendListMessage(listData, callback, asyncState);
        }

        private object[] OnEndSendListMessage(IAsyncResult result)
        {
            this.EndSendListMessage(result);
            return null;
        }

        private void OnSendListMessageCompleted(object state)
        {
            if ((this.SendListMessageCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SendListMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void SendListMessageAsync(APIListMessage listData)
        {
            this.SendListMessageAsync(listData, null);
        }

        public void SendListMessageAsync(APIListMessage listData, object userState)
        {
            if ((this.onBeginSendListMessageDelegate == null))
            {
                this.onBeginSendListMessageDelegate = new BeginOperationDelegate(this.OnBeginSendListMessage);
            }
            if ((this.onEndSendListMessageDelegate == null))
            {
                this.onEndSendListMessageDelegate = new EndOperationDelegate(this.OnEndSendListMessage);
            }
            if ((this.onSendListMessageCompletedDelegate == null))
            {
                this.onSendListMessageCompletedDelegate = new SendOrPostCallback(this.OnSendListMessageCompleted);
            }
            base.InvokeAsync(this.onBeginSendListMessageDelegate, new object[] {
                    listData}, this.onEndSendListMessageDelegate, this.onSendListMessageCompletedDelegate, userState);
        }

        public void SendInfoMessage(APIInfoMessage msg)
        {
            base.Channel.SendInfoMessage(msg);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendInfoMessage(APIInfoMessage msg, AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginSendInfoMessage(msg, callback, asyncState);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public void EndSendInfoMessage(IAsyncResult result)
        {
            base.Channel.EndSendInfoMessage(result);
        }

        private IAsyncResult OnBeginSendInfoMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            APIInfoMessage msg = ((APIInfoMessage)(inValues[0]));
            return this.BeginSendInfoMessage(msg, callback, asyncState);
        }

        private object[] OnEndSendInfoMessage(IAsyncResult result)
        {
            this.EndSendInfoMessage(result);
            return null;
        }

        private void OnSendInfoMessageCompleted(object state)
        {
            if ((this.SendInfoMessageCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SendInfoMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void SendInfoMessageAsync(APIInfoMessage msg)
        {
            this.SendInfoMessageAsync(msg, null);
        }

        public void SendInfoMessageAsync(APIInfoMessage msg, object userState)
        {
            if ((this.onBeginSendInfoMessageDelegate == null))
            {
                this.onBeginSendInfoMessageDelegate = new BeginOperationDelegate(this.OnBeginSendInfoMessage);
            }
            if ((this.onEndSendInfoMessageDelegate == null))
            {
                this.onEndSendInfoMessageDelegate = new EndOperationDelegate(this.OnEndSendInfoMessage);
            }
            if ((this.onSendInfoMessageCompletedDelegate == null))
            {
                this.onSendInfoMessageCompletedDelegate = new SendOrPostCallback(this.OnSendInfoMessageCompleted);
            }
            base.InvokeAsync(this.onBeginSendInfoMessageDelegate, new object[] {
                    msg}, this.onEndSendInfoMessageDelegate, this.onSendInfoMessageCompletedDelegate, userState);
        }

        public void SendDataMessage(APIDataMessage msg)
        {
            base.Channel.SendDataMessage(msg);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendDataMessage(APIDataMessage msg, AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginSendDataMessage(msg, callback, asyncState);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public void EndSendDataMessage(IAsyncResult result)
        {
            base.Channel.EndSendDataMessage(result);
        }

        private IAsyncResult OnBeginSendDataMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            APIDataMessage msg = ((APIDataMessage)(inValues[0]));
            return this.BeginSendDataMessage(msg, callback, asyncState);
        }

        private object[] OnEndSendDataMessage(IAsyncResult result)
        {
            this.EndSendDataMessage(result);
            return null;
        }

        private void OnSendDataMessageCompleted(object state)
        {
            if ((this.SendDataMessageCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SendDataMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void SendDataMessageAsync(APIDataMessage msg)
        {
            this.SendDataMessageAsync(msg, null);
        }

        public void SendDataMessageAsync(APIDataMessage msg, object userState)
        {
            if ((this.onBeginSendDataMessageDelegate == null))
            {
                this.onBeginSendDataMessageDelegate = new BeginOperationDelegate(this.OnBeginSendDataMessage);
            }
            if ((this.onEndSendDataMessageDelegate == null))
            {
                this.onEndSendDataMessageDelegate = new EndOperationDelegate(this.OnEndSendDataMessage);
            }
            if ((this.onSendDataMessageCompletedDelegate == null))
            {
                this.onSendDataMessageCompletedDelegate = new SendOrPostCallback(this.OnSendDataMessageCompleted);
            }
            base.InvokeAsync(this.onBeginSendDataMessageDelegate, new object[] {
                    msg}, this.onEndSendDataMessageDelegate, this.onSendDataMessageCompletedDelegate, userState);
        }

        public void SendMediaPortalMessage(APIMediaPortalMessage msg)
        {
            base.Channel.SendMediaPortalMessage(msg);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendMediaPortalMessage(APIMediaPortalMessage msg, AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginSendMediaPortalMessage(msg, callback, asyncState);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public void EndSendMediaPortalMessage(IAsyncResult result)
        {
            base.Channel.EndSendMediaPortalMessage(result);
        }

        private IAsyncResult OnBeginSendMediaPortalMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            APIMediaPortalMessage msg = ((APIMediaPortalMessage)(inValues[0]));
            return this.BeginSendMediaPortalMessage(msg, callback, asyncState);
        }

        private object[] OnEndSendMediaPortalMessage(IAsyncResult result)
        {
            this.EndSendMediaPortalMessage(result);
            return null;
        }

        private void OnSendMediaPortalMessageCompleted(object state)
        {
            if ((this.SendMediaPortalMessageCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SendMediaPortalMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void SendMediaPortalMessageAsync(APIMediaPortalMessage msg)
        {
            this.SendMediaPortalMessageAsync(msg, null);
        }

        public void SendMediaPortalMessageAsync(APIMediaPortalMessage msg, object userState)
        {
            if ((this.onBeginSendMediaPortalMessageDelegate == null))
            {
                this.onBeginSendMediaPortalMessageDelegate = new BeginOperationDelegate(this.OnBeginSendMediaPortalMessage);
            }
            if ((this.onEndSendMediaPortalMessageDelegate == null))
            {
                this.onEndSendMediaPortalMessageDelegate = new EndOperationDelegate(this.OnEndSendMediaPortalMessage);
            }
            if ((this.onSendMediaPortalMessageCompletedDelegate == null))
            {
                this.onSendMediaPortalMessageCompletedDelegate = new SendOrPostCallback(this.OnSendMediaPortalMessageCompleted);
            }
            base.InvokeAsync(this.onBeginSendMediaPortalMessageDelegate, new object[] {
                    msg}, this.onEndSendMediaPortalMessageDelegate, this.onSendMediaPortalMessageCompletedDelegate, userState);
        }

        public void SendTVServerMessage(APITVServerMessage msg)
        {
            base.Channel.SendTVServerMessage(msg);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginSendTVServerMessage(APITVServerMessage msg, AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginSendTVServerMessage(msg, callback, asyncState);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public void EndSendTVServerMessage(IAsyncResult result)
        {
            base.Channel.EndSendTVServerMessage(result);
        }

        private IAsyncResult OnBeginSendTVServerMessage(object[] inValues, AsyncCallback callback, object asyncState)
        {
            APITVServerMessage msg = ((APITVServerMessage)(inValues[0]));
            return this.BeginSendTVServerMessage(msg, callback, asyncState);
        }

        private object[] OnEndSendTVServerMessage(IAsyncResult result)
        {
            this.EndSendTVServerMessage(result);
            return null;
        }

        private void OnSendTVServerMessageCompleted(object state)
        {
            if ((this.SendTVServerMessageCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SendTVServerMessageCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void SendTVServerMessageAsync(APITVServerMessage msg)
        {
            this.SendTVServerMessageAsync(msg, null);
        }

        public void SendTVServerMessageAsync(APITVServerMessage msg, object userState)
        {
            if ((this.onBeginSendTVServerMessageDelegate == null))
            {
                this.onBeginSendTVServerMessageDelegate = new BeginOperationDelegate(this.OnBeginSendTVServerMessage);
            }
            if ((this.onEndSendTVServerMessageDelegate == null))
            {
                this.onEndSendTVServerMessageDelegate = new EndOperationDelegate(this.OnEndSendTVServerMessage);
            }
            if ((this.onSendTVServerMessageCompletedDelegate == null))
            {
                this.onSendTVServerMessageCompletedDelegate = new SendOrPostCallback(this.OnSendTVServerMessageCompleted);
            }
            base.InvokeAsync(this.onBeginSendTVServerMessageDelegate, new object[] {
                    msg}, this.onEndSendTVServerMessageDelegate, this.onSendTVServerMessageCompletedDelegate, userState);
        }

        public List<APIConnection> Connect(APIConnection name)
        {
            return base.Channel.Connect(name);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginConnect(APIConnection name, AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginConnect(name, callback, asyncState);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public List<APIConnection> EndConnect(IAsyncResult result)
        {
            return base.Channel.EndConnect(result);
        }

        private IAsyncResult OnBeginConnect(object[] inValues, AsyncCallback callback, object asyncState)
        {
            APIConnection name = ((APIConnection)(inValues[0]));
            return this.BeginConnect(name, callback, asyncState);
        }

        private object[] OnEndConnect(IAsyncResult result)
        {
            List<APIConnection> retVal = this.EndConnect(result);
            return new object[] {
                retVal};
        }

        private void OnConnectCompleted(object state)
        {
            if ((this.ConnectCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.ConnectCompleted(this, new ConnectCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }

        public void ConnectAsync(APIConnection name)
        {
            this.ConnectAsync(name, null);
        }

        public void ConnectAsync(APIConnection name, object userState)
        {
            if ((this.onBeginConnectDelegate == null))
            {
                this.onBeginConnectDelegate = new BeginOperationDelegate(this.OnBeginConnect);
            }
            if ((this.onEndConnectDelegate == null))
            {
                this.onEndConnectDelegate = new EndOperationDelegate(this.OnEndConnect);
            }
            if ((this.onConnectCompletedDelegate == null))
            {
                this.onConnectCompletedDelegate = new SendOrPostCallback(this.OnConnectCompleted);
            }
            base.InvokeAsync(this.onBeginConnectDelegate, new object[] {
                    name}, this.onEndConnectDelegate, this.onConnectCompletedDelegate, userState);
        }

        public void Disconnect()
        {
            base.Channel.Disconnect();
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginDisconnect(AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginDisconnect(callback, asyncState);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public void EndDisconnect(IAsyncResult result)
        {
            base.Channel.EndDisconnect(result);
        }

        private IAsyncResult OnBeginDisconnect(object[] inValues, AsyncCallback callback, object asyncState)
        {
            return this.BeginDisconnect(callback, asyncState);
        }

        private object[] OnEndDisconnect(IAsyncResult result)
        {
            this.EndDisconnect(result);
            return null;
        }

        private void OnDisconnectCompleted(object state)
        {
            if ((this.DisconnectCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.DisconnectCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void DisconnectAsync()
        {
            this.DisconnectAsync(null);
        }

        public void DisconnectAsync(object userState)
        {
            if ((this.onBeginDisconnectDelegate == null))
            {
                this.onBeginDisconnectDelegate = new BeginOperationDelegate(this.OnBeginDisconnect);
            }
            if ((this.onEndDisconnectDelegate == null))
            {
                this.onEndDisconnectDelegate = new EndOperationDelegate(this.OnEndDisconnect);
            }
            if ((this.onDisconnectCompletedDelegate == null))
            {
                this.onDisconnectCompletedDelegate = new SendOrPostCallback(this.OnDisconnectCompleted);
            }
            base.InvokeAsync(this.onBeginDisconnectDelegate, null, this.onEndDisconnectDelegate, this.onDisconnectCompletedDelegate, userState);
        }
    }
}