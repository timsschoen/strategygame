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
            
            NetClient = new NetClient(new NetPeerConfiguration("StrategyGame"));
            this.Logger = Logger;
            
            MessagesToSend = new ConcurrentQueue<IMessage>();
            ReceivedMessages = new ConcurrentQueue<IMessage>();
        }

        public bool Connect(string IPAddress, int Port)
        {
            try
            {
                NetClient.Start();
                Connection = NetClient.Connect(IPAddress, Port);
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
                            Logger.Log(LogPriority.Verbose, "Network", "Received Message: " + Message);
                            ReceivedMessages.Enqueue(JsonReader.Deserialize<IMessage>(new JsonTextReader(new StringReader(Message))));
                        }
                        else if(IncomingMessages[i].MessageType == NetIncomingMessageType.ConnectionApproval)
                        {
                            isConnected = true;
                            Logger.Log(LogPriority.Important, "NetworkClient", "Connected to Server");
                        }
                    }

                    IncomingMessages.Clear();

                    //send Messages we need to send
                    IMessage toSend;
                    while(MessagesToSend.TryDequeue(out toSend))
                    {
                        StringWriter stringWriter = new StringWriter();
                        JsonReader.Serialize(new JsonTextWriter(stringWriter), toSend);
                        string Message = stringWriter.ToString();
                        NetClient.SendMessage(NetClient.CreateMessage(Message), NetDeliveryMethod.ReliableOrdered);
                    }
                }

                NetClient.Disconnect("Closed");
            }
        }

        void Stop()
        {
            StopFlag = true;
        }
        
        void INetworkSender.sendOverNetwork(IMessage toSend)
        {
            MessagesToSend.Enqueue(toSend);
        }
    }
}
