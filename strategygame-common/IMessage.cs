namespace strategiespiel_common
{
    interface IMessage
    {
        byte[] GetNetworkBytes();
        bool LoadFromNetworkBytes(byte[] Bytes);
    }
}
