using System;
using System.Net.Sockets;

public class DataReader
{
    private readonly NetworkStream stream;
    private readonly byte[] receiveBuffer = new byte[1024];
    public event Action<DataReader, byte[], int> ReadDataAsyncEvent;
    public event Action<DataReader> ReadDisconnectAsyncEvent;

    public DataReader(NetworkStream stream)
    {
        this.stream = stream;
        this.stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, OnRead, null);
    }

    private void OnRead(IAsyncResult asyncResult)
    {
        Logger.Log("DataReader OnRead");
        var byteLength = stream?.EndRead(asyncResult) ?? 0;
        if (byteLength <= 0)
        {
            ReadDisconnectAsyncEvent?.Invoke(this);
            return;
        }

        ReadDataAsyncEvent?.Invoke(this, receiveBuffer, byteLength);

        stream?.BeginRead(receiveBuffer, 0, receiveBuffer.Length, OnRead, null);
    }
}