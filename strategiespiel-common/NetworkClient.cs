using System;
using System.Net.Sockets;
using System.Threading;

namespace strategiespiel_common
{
    class NetworkClient : INetworkSender
    {
        Thread clientThread;
        TcpClient tcpClient;

        public NetworkClient()
        {
            clientThread = new Thread(new ThreadStart(NetworkUpdateLoop));
        }

        public void Connect(string IPAddress)
        {
            //TODO
        }

        void NetworkUpdateLoop()
        {
            //TODO
        }

        void INetworkSender.addOnMessageReceivedHandler(NetwokMessageHandler handler, string listenForMessageType)
        {
            throw new NotImplementedException();
        }

        void INetworkSender.sendOverNetwork(Message toSend)
        {
            throw new NotImplementedException();
        }
    }
}
