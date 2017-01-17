using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Lidgren.Network;

namespace strategygame_common
{
    public class NetworkServer
    {
        private Thread serverThread;

        private Dictionary<int, Client> connectedClients;
        private Dictionary<NetConnection, int> Connections;

        private ConcurrentQueue<IMessage> IncomingMessageQueue;
        private ConcurrentQueue<IMessage> OutgoingMessageQueue;
        NetServer LidgrenServer;

        const string version = "0.1";
        int NextID = 0;
        ILogger Logger;
        bool StopFlag = false;

        public NetworkServer(ILogger Logger)
        {
            serverThread = new Thread(new ThreadStart(NetworkUpdateLoop));
            IncomingMessageQueue = new ConcurrentQueue<IMessage>();
            OutgoingMessageQueue = new ConcurrentQueue<IMessage>();
            connectedClients = new Dictionary<int, Client>();
            Connections = new Dictionary<NetConnection, int>();
            this.Logger = Logger;
        }

        public IMessage TryGetNewMessage()
        {
            IMessage message = null;
            if (IncomingMessageQueue.TryDequeue(out message))
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
            NetPeerConfiguration config = new NetPeerConfiguration("StrategyGame");
            LidgrenServer = new NetServer(config);
            LidgrenServer.Start();

            while(!StopFlag)
            {
                NetIncomingMessage incMsg = LidgrenServer.ReadMessage();

                if(incMsg.MessageType == NetIncomingMessageType.StatusChanged)
                {
                    if(incMsg.SenderConnection.Status == NetConnectionStatus.Connected)
                    {
                        Client client = new Client(incMsg.SenderConnection);
                        Connections.Add(incMsg.SenderConnection, NextID);
                        connectedClients.Add(NextID, client);
                        Logger.Log(LogPriority.Important, "Network", "New Client " + NextID + " connected.");
                        IncomingMessageQueue.Enqueue(new NewClientConnectedMessage(NextID));
                        NextID++;
                    }
                }
                else if(incMsg.MessageType == NetIncomingMessageType.Data)
                {
                    if (!Connections.ContainsKey(incMsg.SenderConnection))
                        continue;

                    handleMessage(incMsg, Connections[incMsg.SenderConnection]);
                }
                    
            }
        }        

        void handleMessage(NetIncomingMessage Message, int ClientID)
        {

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
            public NetConnection Connection;

            public Client(NetConnection Connection)
            {
                this.Connection = Connection;
            }
        }

        public class RawMessage : IMessage
        {
            public int ClientID { get; set; }

            byte[] content;

            public byte[] GetNetworkBytes()
            {
                return content;
            }

            public bool LoadFromNetworkBytes(byte[] content, int clientID)
            {
                this.content = content;
                this.ClientID = clientID;
                return true;
            }
        }

        public class NewClientConnectedMessage : IMessage
        {
            public int ClientID { get; set; }

            public byte[] GetNetworkBytes()
            {
                return null;
            }

            public bool LoadFromNetworkBytes(byte[] content, int clientID)
            {
                return false;
            }

            public NewClientConnectedMessage(int ClientID)
            {
                this.ClientID = ClientID;
            }
        }
    }
}