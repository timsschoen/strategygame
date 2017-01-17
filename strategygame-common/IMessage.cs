namespace strategygame_common
{
    public interface IMessage
    {
        byte[] GetNetworkBytes();
        bool LoadFromNetworkBytes(byte[] content, int clientID);

        int ClientID { get; set; }
    }
}
