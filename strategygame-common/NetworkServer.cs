using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace strategygame_common
{
    public class NetworkServer
    {
        private Thread serverThread;
        private List<Client> connectedClients;
        private ConcurrentQueue<RawMessage> messageQueue;

        public NetworkServer()
        {
            serverThread = new Thread(new ThreadStart(NetworkUpdateLoop));
            messageQueue = new ConcurrentQueue<RawMessage>();
        }

        public RawMessage TryGetNewMessage()
        {
            RawMessage message = null;
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

        void addOnMessageReceivedHandler(NetwokMessageHandler handler, string listenForMessageType)
        {
            throw new NotImplementedException();
        }

        void sendOverNetwork(IMessage toSend)
        {
            throw new NotImplementedException();
        }

        public class Client
        {
            public TcpClient tcpClient;
        }

        public class RawMessage : IMessage
        {
            Client NetworkClient;

            public byte[] GetNetworkBytes()
            {
                throw new NotImplementedException();
            }

            public bool LoadFromNetworkBytes(byte[] Bytes)
            {
                throw new NotImplementedException();
            }
        }
    }
}