namespace strategiespiel_common
{
    delegate void NetwokMessageHandler(int fromID, Message message);

    interface INetworkSender
    {
        void sendOverNetwork(Message toSend);
        void addOnMessageReceivedHandler(NetwokMessageHandler handler, string listenForMessageType);        
    }
}
