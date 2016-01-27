using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace strategygame_common
{
    class NetworkServer : INetworkSender
    {
        private Thread serverThread;
        private List<Client> connectedClients;
        private ConcurrentQueue<IMessage> messageQueue;

        public NetworkServer()
        {
            serverThread = new Thread(new ThreadStart(NetworkUpdateLoop));
            messageQueue = new ConcurrentQueue<IMessage>();
        }

        public IMessage TryGetNewMessage()
        {
            IMessage message = null;
            if (messageQueue.TryDequeue(out message))
                return message;
            else
                return null;
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

        void INetworkSender.sendOverNetwork(IMessage toSend)
        {
            throw new NotImplementedException();
        }

        private class Client
        {
            public TcpClient tcpClient;
            public int ID;
        }
    }
}
