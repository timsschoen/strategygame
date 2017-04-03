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
        private Thread mServerThread;

        private Dictionary<int, Client> mConnectedClients;
        private Dictionary<NetConnection, int> mConnections;

        private ConcurrentQueue<IMessage> mIncomingMessageQueue;
        private ConcurrentQueue<IMessage> mOutgoingMessageQueue;
        NetServer mLidgrenServer;

        const string mVersion = "0.1";
        int mNextID = 0;
        ILogger mLogger;
        volatile bool mStopFlag = false;

        int mPort;

        /// <summary>
        /// Constructs a new network server
        /// </summary>
        /// <param name="Logger">ILogger to log to</param>
        /// <param name="Port">Port to listen on, default 6679</param>
        public NetworkServer(ILogger Logger, int Port = 6679)
        {
            mServerThread = new Thread(new ThreadStart(NetworkUpdateLoop));
            mIncomingMessageQueue = new ConcurrentQueue<IMessage>();
            mOutgoingMessageQueue = new ConcurrentQueue<IMessage>();
            mConnectedClients = new Dictionary<int, Client>();
            mConnections = new Dictionary<NetConnection, int>();

            mLogger = Logger;
            mPort = Port;
        }

        public IMessage TryGetNewMessage()
        {
            IMessage message = null;
            if (mIncomingMessageQueue.TryDequeue(out message))
                return message;
            else
                return null;
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        public void Start()
        {
            mServerThread.Start();
        }

        /// <summary>
        /// loops until mStopFlag is false
        /// </summary>
        void NetworkUpdateLoop()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("StrategyGame");
            config.Port = mPort;
            config.SetMessageTypeEnabled(NetIncomingMessageType.DebugMessage, true);
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, true);
            mLidgrenServer = new NetServer(config);
            mLidgrenServer.Start();
            mLogger.Log(LogPriority.Important, "NetworkServer", "Server started.");

            JsonSerializer JsonReader = new JsonSerializer();
            List<NetIncomingMessage> IncomingMessages = new List<NetIncomingMessage>();

            while (!mStopFlag)
            {
                mLidgrenServer.ReadMessages(IncomingMessages);

                for(int i = 0; i < IncomingMessages.Count; i++)
                {
                    HandleMessage(IncomingMessages[i]);
                }

                IncomingMessages.Clear();

                //send Messages we need to send
                IMessage toSend;
                while (mOutgoingMessageQueue.TryDequeue(out toSend))
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.TypeNameHandling = TypeNameHandling.Objects;
                    string Message = JsonConvert.SerializeObject(toSend, settings);
                    NetOutgoingMessage outMsg = mLidgrenServer.CreateMessage(Message);

                    if (toSend.ClientID == -1)
                    {
                        //Send to all
                        foreach (KeyValuePair<int, Client> KVP in mConnectedClients)
                        {
                            mLidgrenServer.SendMessage(outMsg, KVP.Value.Connection, NetDeliveryMethod.ReliableOrdered);
                        }
                    }
                    else
                    {
                        mLidgrenServer.SendMessage(outMsg, mConnectedClients[toSend.ClientID].Connection, NetDeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }        

        /// <summary>
        /// Handles the received network message
        /// </summary>
        /// <param name="incomingMessage">message to handle</param>
        void HandleMessage(NetIncomingMessage incomingMessage)
        {
            if(incomingMessage.MessageType == NetIncomingMessageType.ConnectionApproval)
            {
                incomingMessage.SenderConnection.Approve();
            }
            else if (incomingMessage.MessageType == NetIncomingMessageType.StatusChanged)
            {
                if (incomingMessage.SenderConnection.Status == NetConnectionStatus.Connected)
                {
                    Client client = new Client(incomingMessage.SenderConnection);
                    mConnections.Add(incomingMessage.SenderConnection, mNextID);
                    mConnectedClients.Add(mNextID, client);
                    mLogger.Log(LogPriority.Important, "Network", "New Client " + mNextID + " connected.");
                    mIncomingMessageQueue.Enqueue(new NewClientConnectedMessage(mNextID));
                    mNextID++;
                }
                else if(incomingMessage.SenderConnection.Status == NetConnectionStatus.Disconnected || incomingMessage.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                {
                    if (!mConnections.ContainsKey(incomingMessage.SenderConnection))
                        return;

                    int id = mConnections[incomingMessage.SenderConnection];
                    mConnections.Remove(incomingMessage.SenderConnection);
                    mConnectedClients.Remove(id);
                    mLogger.Log(LogPriority.Important, "Network", "Client " + mNextID + " disconnected.");
                }
            }
            else if (incomingMessage.MessageType == NetIncomingMessageType.Data)
            {
                if (!mConnections.ContainsKey(incomingMessage.SenderConnection))
                    return;

                HandleConnectedMessage(incomingMessage, mConnections[incomingMessage.SenderConnection]);
            }
        }

        /// <summary>
        /// handles game-messages from connected clients
        /// </summary>
        /// <param name="incomingConnectedMessage">the received game-message</param>
        /// <param name="senderClientID">id of the client, fromwhich the message was received</param>
        void HandleConnectedMessage(NetIncomingMessage incomingConnectedMessage, int senderClientID)
        {
            string Message = incomingConnectedMessage.ReadString();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            IMessage ParsedMessage = (IMessage)JsonConvert.DeserializeObject(Message, settings);
            ParsedMessage.ClientID = senderClientID;
            mIncomingMessageQueue.Enqueue(ParsedMessage);
        }
         
        /// <summary>
        /// Send the given game-message to the client specified in the message
        /// </summary>
        /// <param name="messageToSend">message to send, toSend.ClientID will determine which client to send the message to, -1 for all clients</param>
        public void SendOverNetwork(IMessage messageToSend)
        {
            mOutgoingMessageQueue.Enqueue(messageToSend);
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

    public class ClientLoadedMessage : BaseMessage
    { }
}