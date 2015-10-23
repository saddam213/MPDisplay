using System;
using System.Net.Security;
using System.ServiceModel;

namespace Common.Helpers
{
    public static class ConnectHelper
    {
        public static NetTcpBinding GetServerBinding()
        {
            var serverBinding = new NetTcpBinding
            {
                Security =
                {
                    Mode = SecurityMode.None,
                    Message = {ClientCredentialType = MessageCredentialType.None},
                    Transport =
                    {
                        ClientCredentialType = TcpClientCredentialType.None,
                        ProtectionLevel = ProtectionLevel.None
                    }
                },
                Name = "NetTcpBinding_IMessage",
                CloseTimeout = new TimeSpan(0, 0, 5),
                OpenTimeout = new TimeSpan(0, 0, 5),
                ReceiveTimeout = new TimeSpan(0, 1, 0),
                SendTimeout = new TimeSpan(0, 1, 0),
                TransferMode = TransferMode.Buffered,
                ListenBacklog = 100,
                MaxConnections = 100,
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                ReaderQuotas =
                {
                    MaxArrayLength = int.MaxValue,
                    MaxDepth = 32,
                    MaxStringContentLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue
                },
                ReliableSession = {Enabled = true, InactivityTimeout = new TimeSpan(0, 3, 0)}
            };

            return serverBinding;
        }

    }
}
