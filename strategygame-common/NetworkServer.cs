using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Lidgren.Network;
using System.IO;
using Newtonsoft.Json;

namespace strategygame_common
{
    public class NetworkServer : INetworkSender
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
            config.Port = 6679;
            config.AcceptIncomingConnections = true;
            LidgrenServer = new NetServer(config);
            LidgrenServer.Start();
            Logger.Log(LogPriority.Important, "NetworkServer", "Server started.");

            JsonSerializer JsonReader = new JsonSerializer();
            List<NetIncomingMessage> IncomingMessages = new List<NetIncomingMessage>();

            while (!StopFlag)
            {
                LidgrenServer.ReadMessages(IncomingMessages);

                for(int i = 0; i < IncomingMessages.Count; i++)
                {
                    handleMessage(IncomingMessages[i]);
                }

                IncomingMessages.Clear();

                //send Messages we need to send
                IMessage toSend;
                while (OutgoingMessageQueue.TryDequeue(out toSend))
                {
                    StringWriter stringWriter = new StringWriter();
                    JsonReader.Serialize(new JsonTextWriter(stringWriter), toSend);
                    string Message = stringWriter.ToString();
                    LidgrenServer.SendMessage(LidgrenServer.CreateMessage(Message), connectedClients[toSend.ClientID].Connection, NetDeliveryMethod.ReliableOrdered);
                }

            }
        }        

        void handleMessage(NetIncomingMessage incMsg)
        {
            if (incMsg.MessageType == NetIncomingMessageType.StatusChanged)
            {
                if (incMsg.SenderConnection.Status == NetConnectionStatus.Connected)
                {
                    Client client = new Client(incMsg.SenderConnection);
                    Connections.Add(incMsg.SenderConnection, NextID);
                    connectedClients.Add(NextID, client);
                    Logger.Log(LogPriority.Important, "Network", "New Client " + NextID + " connected.");
                    IncomingMessageQueue.Enqueue(new NewClientConnectedMessage(NextID));
                    NextID++;
                }
                else if(incMsg.SenderConnection.Status == NetConnectionStatus.Disconnected || incMsg.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                {
                    if (!Connections.ContainsKey(incMsg.SenderConnection))
                        return;

                    int id = Connections[incMsg.SenderConnection];
                    Connections.Remove(incMsg.SenderConnection);
                    connectedClients.Remove(id);
                    Logger.Log(LogPriority.Important, "Network", "Client " + NextID + " disconnected.");
                }
            }
            else if (incMsg.MessageType == NetIncomingMessageType.Data)
            {
                if (!Connections.ContainsKey(incMsg.SenderConnection))
                    return;

                handleConnectedMessage(incMsg, Connections[incMsg.SenderConnection]);
            }
        }

        void handleConnectedMessage(NetIncomingMessage Message, int ClientID)
        {

        }
         
        public void sendOverNetwork(IMessage toSend)
        {
            throw new NotImplementedException();
        }
    }

    public class Client
    {
        public NetConnection Connection;

        public Client(NetConnection Connection)
        {
            this.Connection = Connection;
        }
    }

    public class NewClientConnectedMessage : BaseMessage
    {
        public NewClientConnectedMessage(int ClientID)
        {
            this.ClientID = ClientID;
        }
    }
}