namespace strategygame_common
{
    public interface IMessage
    {
        byte[] GetNetworkBytes();
        bool LoadFromNetworkBytes(byte[] Bytes);
    }
}
