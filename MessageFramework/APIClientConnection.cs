using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using MessageFramework.DataObjects;

namespace MessageFramework
{
    //[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)] 
    //public class APIClientConnection : IMessageCallback
    //{
    //    private APIConnection _connection;
    //    private MessageClient _messageClient;
    //    private EndpointAddress _serverEndpoint;
    //    private NetTcpBinding _serverBinding;
    //    private InstanceContext _callbackInstance;

    //    public APIClientConnection()
    //    {

    //    }

    //    public void InitializeConnection(string ip, int port, string name)
    //    {
    //        _serverEndpoint = new EndpointAddress(string.Format("net.tcp://{0}:{1}/MPDisplayService", ip, port));
    //        _serverBinding = new NetTcpBinding();

    //        // Security (lol)
    //        _serverBinding.Security.Mode = SecurityMode.None;
    //        _serverBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
    //        _serverBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
    //        _serverBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

    //        // Connection
    //        _serverBinding.Name = "NetTcpBinding_IMessage";
    //        _serverBinding.CloseTimeout = new TimeSpan(0, 10, 0);
    //        _serverBinding.OpenTimeout = new TimeSpan(0, 10, 0);
    //        _serverBinding.ReceiveTimeout = new TimeSpan(7, 0, 0, 0);//7 days should be enough :)
    //        _serverBinding.SendTimeout = new TimeSpan(0, 10, 0);
    //        _serverBinding.TransferMode = TransferMode.Buffered;
    //        _serverBinding.ListenBacklog = 1;
    //        _serverBinding.MaxConnections = 10;
    //        _serverBinding.MaxReceivedMessageSize = 1000000;
    //        _serverBinding.MaxBufferSize = 1000000;
    //        _serverBinding.MaxBufferPoolSize = 1000000;

    //        // Message
    //        _serverBinding.ReaderQuotas.MaxArrayLength = 1000000;
    //        _serverBinding.ReaderQuotas.MaxDepth = 32;
    //        _serverBinding.ReaderQuotas.MaxStringContentLength = 1000000;
    //        _serverBinding.ReaderQuotas.MaxBytesPerRead = 1000000;
    //        _serverBinding.ReliableSession.Enabled = true;
    //        _serverBinding.ReliableSession.InactivityTimeout = new TimeSpan(7, 0, 0, 0);//7 days should be enough :)

    //        _connection = new APIConnection(name);
    //        _callbackInstance = new InstanceContext(this);
    //    }

    //    public void Connect()
    //    {
    //        try
    //        {
    //            _messageClient = new MessageClient(_callbackInstance, _serverBinding, _serverEndpoint);
    //            _messageClient.InnerChannel.Faulted += new EventHandler(ConnectionChannel_Faulted);
    //            _messageClient.ConnectCompleted += new EventHandler<ConnectCompletedEventArgs>(Connect_Completed);
    //            _messageClient.DisconnectCompleted += new EventHandler<AsyncCompletedEventArgs>(Disconnect_Completed);
    //            _messageClient.ConnectAsync(_connection);
    //        }
    //        catch { }
    //    }

    //    public void Disconnect()
    //    {
    //        try
    //        {
    //            _messageClient.Disconnect();
    //            _messageClient.InnerChannel.Faulted -= ConnectionChannel_Faulted;
    //            _messageClient.ConnectCompleted -= Connect_Completed;
    //            _messageClient.DisconnectCompleted -= Disconnect_Completed;
    //            _messageClient = null;
    //        }
    //        catch { }
    //        OnDisconnected();
    //    }

    //    public virtual void OnConnected(APIConnection[] connections)
    //    {

    //    }

    //    public virtual void OnDisconnected()
    //    {

    //    }



    //    #region IMessageCallback


    //    public virtual void ReceiveAPIPropertyMessage(APIPropertyMessage message)
    //    {
           
    //    }

    //    public virtual void ReceiveAPIListMessage(APIListMessage message)
    //    {
           
    //    }

    //    public virtual void ReceiveAPIInfoMessage(APIInfoMessage message)
    //    {
           
    //    }

    //    public virtual void ReceiveAPIVisibleMessage(APIVisibleMessage message)
    //    {
           
    //    }

    //    public virtual void ReceiveAPIDataMessage(APIDataMessage message)
    //    {
           
    //    }

    //    public virtual void ReceiveMediaPortalMessage(APIMediaPortalMessage message)
    //    {
           
    //    }

    //    public virtual void ReceiveTVServerMessage(APITVServerMessage message)
    //    {
            
    //    }

    //    public virtual void SessionConnected(APIConnection connection)
    //    {
           
    //    }

    //    public virtual void SessionDisconnected(APIConnection connection)
    //    {
            
    //    }

    //    #endregion

    //    private void Connect_Completed(object sender, ConnectCompletedEventArgs e)
    //    {
    //        if (e.Error == null)
    //        {
    //            OnConnected(e.Result);
    //        }
    //    }

    //    private void Disconnect_Completed(object sender, AsyncCompletedEventArgs e)
    //    {
    //        OnDisconnected();
    //    }

    //    private void ConnectionChannel_Faulted(object sender, EventArgs e)
    //    {
    //        Disconnect();
    //    }
    //}
}
