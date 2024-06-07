using System;
using System.Net.Sockets;
using System.Threading;

public class ConnectionDataTeller
{
    private readonly SynchronizationContext syncContext;
    private readonly NetworkStream stream;
    private readonly int dataBufferSize = 1024;
    private readonly byte[] receiveBuffer;
    public event Action<ConnectionDataTeller, byte[]> ReadDataEvent;
    public event Action<ConnectionDataTeller> ReadDisconnectEvent;

    public ConnectionDataTeller(NetworkStream stream)
    {
        syncContext = SynchronizationContext.Current;
        this.stream = stream;
        this.stream.BeginRead(receiveBuffer, 0, dataBufferSize, OnRead, null);
    }

    private void OnRead(IAsyncResult asyncResult)
    {
        var byteLength = stream?.EndRead(asyncResult) ?? 0;
        if (byteLength <= 0)
        {
            syncContext.Post(_ => ReadDisconnectEvent?.Invoke(this), null);
            return;
        }

        var data = new byte[byteLength];
        Array.Copy(receiveBuffer, data, byteLength);
        syncContext.Post(_ => ReadDataEvent?.Invoke(this, data), null);
        
        stream?.BeginRead(receiveBuffer, 0, dataBufferSize, OnRead, null);
    }
}