using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Common.Helpers
{
    public static class ConnectHelper
    {
        public static NetTcpBinding getServerBinding()
        {
            NetTcpBinding _serverBinding;

            _serverBinding = new NetTcpBinding();

            // Security (lol)
            _serverBinding.Security.Mode = SecurityMode.None;
            _serverBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            _serverBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            _serverBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

            // Connection
            _serverBinding.Name = "NetTcpBinding_IMessage";
            _serverBinding.CloseTimeout = new TimeSpan(0, 0, 5);
            _serverBinding.OpenTimeout = new TimeSpan(0, 0, 5);
            _serverBinding.ReceiveTimeout = new TimeSpan(0, 1, 0);
            _serverBinding.SendTimeout = new TimeSpan(0, 1, 0);
            _serverBinding.TransferMode = TransferMode.Buffered;
            _serverBinding.ListenBacklog = 100;
            _serverBinding.MaxConnections = 100;
            _serverBinding.MaxReceivedMessageSize = int.MaxValue;
            _serverBinding.MaxBufferSize = int.MaxValue;
            _serverBinding.MaxBufferPoolSize = int.MaxValue;

            // Message
            _serverBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            _serverBinding.ReaderQuotas.MaxDepth = 32;
            _serverBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            _serverBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            _serverBinding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            _serverBinding.ReliableSession.Enabled = true;
            _serverBinding.ReliableSession.InactivityTimeout = new TimeSpan(0, 3, 0);

            return _serverBinding;
        }

    }
}
