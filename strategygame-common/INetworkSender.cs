namespace strategygame_common
{

    public interface INetworkSender
    {
        void SendOverNetwork(IMessage toSend);     
    }
}
