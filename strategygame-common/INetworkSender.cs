namespace strategygame_common
{

    public interface INetworkSender
    {
        void sendOverNetwork(IMessage toSend);     
    }
}
