using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Lidgren.Network;
using Newtonsoft.Json;
using System.IO;

namespace strategygame_common
{
    public class NetworkClient : INetworkSender
    {
        Thread clientThread;
        NetClient NetClient;

        public bool isConnected { get; private set; } = false;
        public bool StopFlag { get; private set; }
        private NetConnection Connection;

        private ConcurrentQueue<IMessage> MessagesToSend;
        private ConcurrentQueue<IMessage> ReceivedMessages;
        private ILogger Logger;

        public NetworkClient(ILogger Logger)
        {
            clientThread = new Thread(new ThreadStart(NetworkUpdateLoop));

            NetPeerConfiguration config = new NetPeerConfiguration("StrategyGame");
            config.SetMessageTypeEnabled(NetIncomingMessageType.DebugMessage, true);
            NetClient = new NetClient(config);
            this.Logger = Logger;
            
            MessagesToSend = new ConcurrentQueue<IMessage>();
            ReceivedMessages = new ConcurrentQueue<IMessage>();
        }

        public bool Connect(string IPAddress, int Port)
        {
            try
            {
                NetClient.Start();
                
                Connection = NetClient.Connect(IPAddress, Port, NetClient.CreateMessage("test"));
                if (clientThread.ThreadState == ThreadState.Unstarted || clientThread.ThreadState == ThreadState.Stopped)
                    clientThread.Start();


                return true;
            }
            catch(Exception ex)
            {
                isConnected = false;
                Logger.Log(LogPriority.Important, "NetworkClient", "Could not connect " + ex.Message);
                return false;
            }
        }

        void NetworkUpdateLoop()
        {
            List<NetIncomingMessage> IncomingMessages = new List<NetIncomingMessage>();
            JsonSerializer JsonReader = new JsonSerializer();
            while(!StopFlag && Thread.CurrentThread.ThreadState == ThreadState.Running)
            {
                if(NetClient != null)
                { 
                    //look if we got any messages
                    NetClient.ReadMessages(IncomingMessages);

                    for (int i = 0; i < IncomingMessages.Count; i++)
                    {
                        if (IncomingMessages[i].MessageType == NetIncomingMessageType.Data)
                        {
                            string Message = IncomingMessages[i].ReadString();
                            JsonSerializerSettings settings = new JsonSerializerSettings();
                            settings.TypeNameHandling = TypeNameHandling.Objects;
                            ReceivedMessages.Enqueue((IMessage)JsonConvert.DeserializeObject(Message,settings));
                        }
                        else if(IncomingMessages[i].MessageType == NetIncomingMessageType.StatusChanged)
                        {
                            if (IncomingMessages[i].SenderConnection.Status == NetConnectionStatus.InitiatedConnect || IncomingMessages[i].SenderConnection.Status == NetConnectionStatus.Connected)
                            {
                                isConnected = true;
                                Logger.Log(LogPriority.Important, "NetworkClient", "Connected to Server");
                            }
                        }
                        else if(IncomingMessages[i].MessageType == NetIncomingMessageType.DebugMessage 
                            || IncomingMessages[i].MessageType == NetIncomingMessageType.VerboseDebugMessage
                            || IncomingMessages[i].MessageType == NetIncomingMessageType.Error
                            || IncomingMessages[i].MessageType == NetIncomingMessageType.ErrorMessage)
                        {
                            Logger.Log(LogPriority.Important, "NetworkClient", IncomingMessages[i].ReadString()); 
                        }
                    }

                    IncomingMessages.Clear();

                    //send Messages we need to send
                    IMessage toSend;
                    while(MessagesToSend.TryDequeue(out toSend))
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        settings.TypeNameHandling = TypeNameHandling.Objects;
                        string Message = JsonConvert.SerializeObject(toSend, settings);
                        NetClient.SendMessage(NetClient.CreateMessage(Message), NetDeliveryMethod.ReliableOrdered);
                    }
                }
            }
            NetClient.Disconnect("Closed");
        }

        public IMessage TryGetMessage()
        {
            IMessage Result;
            if (!ReceivedMessages.TryDequeue(out Result))
                return null;

            return Result;
        }

        public void Stop()
        {
            StopFlag = true;
        }
        
        public void sendOverNetwork(IMessage toSend)
        {
            MessagesToSend.Enqueue(toSend);
        }
    }
}
