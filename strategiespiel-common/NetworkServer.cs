using System;
using System.Threading;

namespace strategiespiel_common
{
    class NetworkServer : INetworkSender
    {
        private Thread serverThread;

        public NetworkServer()
        {
            serverThread = new Thread(new ThreadStart(NetworkUpdateLoop));
        }

        public void Start()
        {
            serverThread.Start();
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
