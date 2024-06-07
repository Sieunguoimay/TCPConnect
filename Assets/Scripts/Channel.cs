using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Channel //: IDisposable
{
    public StreamWriter Writer { get; }
    private readonly int dataBufferSize = 1024;
    private readonly byte[] receiveBuffer;
    private readonly NetworkStream stream;
    protected SynchronizationContext SyncContext { get; }

    // public event Action<Channel> DisconnectedEvent;
    public event Action<Channel, byte[]> ReceivedDataEvent;
    private readonly Action<Channel> disconnectCallback;

    public Channel(NetworkStream stream, Action<Channel> disconnectCallback)
    {
        this.stream = stream;
        this.disconnectCallback = disconnectCallback;
        if (this.stream != null)
        {
            SyncContext = SynchronizationContext.Current;
            Writer = new StreamWriter(this.stream, Encoding.ASCII);
            receiveBuffer = new byte[dataBufferSize];
            this.stream.BeginRead(receiveBuffer, 0, dataBufferSize, OnRead, null);

            Debug.Log($"Created Connection {this.stream}");
        }
        else
        {
            Debug.Log($"Failed to create Channel. Client is not connected.");
        }
    }

    private void OnRead(IAsyncResult asyncResult)
    {
        int byteLength = stream?.EndRead(asyncResult) ?? 0;
        if (byteLength <= 0)
        {
            // Disconnect client
            SyncContext.Post(_ => disconnectCallback?.Invoke(this), null);
            return;
        }

        // Transfer data from receiveBuffer to data variable for handling
        var data = new byte[byteLength];
        Array.Copy(receiveBuffer, data, byteLength);
        // Handle data in any way you want to

        // BeginRead again so you can keep receiving data
        stream?.BeginRead(receiveBuffer, 0, dataBufferSize, OnRead, null);
    }

    // public void Dispose()
    // {
    //     _stream.Close();
    //     Debug.Log($"Connection: Disconnect. Client.Connected = {Client.Connected}");
    //     _stream = null;

    //     DisconnectedEvent?.Invoke(this);
    // }

    protected virtual void HandleReadData(byte[] data)
    {
        SyncContext.Post(_ => ReceivedDataEvent?.Invoke(this, data), null);
    }
}
