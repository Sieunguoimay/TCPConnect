using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Channel: IDisposable
{
    public StreamWriter Writer { get; }
    public TcpClient Client { get; }
    private readonly int dataBufferSize = 1024;
    private readonly byte[] receiveBuffer;
    private NetworkStream _stream;
    private readonly SynchronizationContext _syncContext;

    public event Action<Channel> DisconnectedEvent;
    public event Action<Channel, byte[]> ReceivedDataEvent;
    private readonly Action<Channel> disconnectCallback;

    public Channel(TcpClient client, Action<Channel> disconnectCallback)
    {
        Client = client;
        this.disconnectCallback = disconnectCallback;
        if (Client.Connected)
        {
            _syncContext = SynchronizationContext.Current;
            _stream = Client.GetStream();
            Writer = new StreamWriter(_stream, Encoding.ASCII);
            receiveBuffer = new byte[dataBufferSize];
            _stream.BeginRead(receiveBuffer, 0, dataBufferSize, OnRead, null);

            Debug.Log($"Created Connection {_stream}");
        }
        else
        {
            Debug.Log($"Failed to create Channel. Client is not connected.");
        }
    }

    private void OnRead(IAsyncResult asyncResult)
    {
        int byteLength = _stream?.EndRead(asyncResult) ?? 0;
        if (byteLength <= 0)
        {
            // Disconnect client
            _syncContext.Post(_ => disconnectCallback?.Invoke(this), null);
            return;
        }

        // Transfer data from receiveBuffer to data variable for handling
        var data = new byte[byteLength];
        Array.Copy(receiveBuffer, data, byteLength);
        // Handle data in any way you want to

        _syncContext.Post(_ => ReceivedDataEvent?.Invoke(this, data), null);

        // BeginRead again so you can keep receiving data
        _stream?.BeginRead(receiveBuffer, 0, dataBufferSize, OnRead, null);
    }

    public void Dispose()
    {
        _stream.Close();
        Debug.Log($"Connection: Disconnect. Client.Connected = {Client.Connected}");
        _stream = null;

        DisconnectedEvent?.Invoke(this);
    }
}
