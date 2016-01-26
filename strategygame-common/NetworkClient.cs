using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace strategiespiel_common
{
    class NetworkClient : INetworkSender
    {
        Thread clientThread;
        TcpClient tcpClient;

        public bool isConnected { get; private set; } = false;

        private ConcurrentQueue<IMessage> MessagesToSend;
        private ConcurrentQueue<IMessage> ReceivedMessages;
        
        //TODO Thread Safety
        private Lookup<string, NetwokMessageHandler> HandlersForMessageType;

        public NetworkClient()
        {
            clientThread = new Thread(new ThreadStart(NetworkUpdateLoop));
            tcpClient = new TcpClient();

            //HandlersForMessageType = Lookup< string, NetwokMessageHandler >

            MessagesToSend = new ConcurrentQueue<IMessage>();
            ReceivedMessages = new ConcurrentQueue<IMessage>();
        }

        public bool Connect(string IPAddress, int Port)
        {
            try
            {
                tcpClient.Connect(IPAddress, Port);
                isConnected = true;
                return true;
            }
            catch(Exception ex)
            {
                isConnected = false;
                return false;
            }
        }

        void NetworkUpdateLoop()
        {
            //TODO
        }

        void INetworkSender.addOnMessageReceivedHandler(NetwokMessageHandler handler, string listenForMessageType)
        {
            //HandlersForMessageType.
        }

        void INetworkSender.sendOverNetwork(IMessage toSend)
        {
            MessagesToSend.Enqueue(toSend);
        }
    }
}
