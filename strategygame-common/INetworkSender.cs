namespace strategygame_common
{
    delegate void NetwokMessageHandler(int fromID, IMessage message);

    interface INetworkSender
    {
        void sendOverNetwork(IMessage toSend);     
    }
}
