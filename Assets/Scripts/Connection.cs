using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Connection
{
    public StreamWriter Writer { get; }
    public TcpClient Client { get; }
    private readonly int dataBufferSize = 1024;
    private readonly byte[] receiveBuffer;
    private NetworkStream _stream;

    public event Action<Connection> DisconnectedEvent;
    public event Action<Connection, byte[]> ReceivedDataEvent;

    public Connection(TcpClient client)
    {
        Client = client;
        _stream = Client.GetStream();
        Debug.Log($"Created Connection {_stream}");
        Writer = new StreamWriter(_stream, Encoding.ASCII);
        receiveBuffer = new byte[dataBufferSize];
        _stream.BeginRead(receiveBuffer, 0, dataBufferSize, OnRead, null);
    }

    private void OnRead(IAsyncResult asyncResult)
    {
        int byteLength = _stream?.EndRead(asyncResult) ?? 0;
        if (byteLength <= 0)
        {
            // Disconnect client
            Disconnect();
            return;
        }

        // Transfer data from receiveBuffer to data variable for handling
        var data = new byte[byteLength];
        Array.Copy(receiveBuffer, data, byteLength);
        // Handle data in any way you want to

        ReceivedDataEvent?.Invoke(this, data);

        // BeginRead again so you can keep receiving data
        _stream?.BeginRead(receiveBuffer, 0, dataBufferSize, OnRead, null);
    }

    public void Disconnect()
    {
        _stream.Close();
        Debug.Log($"Connection: Disconnect. Client.Connected = {Client.Connected}");
        _stream = null;
        DisconnectedEvent?.Invoke(this);
    }
}