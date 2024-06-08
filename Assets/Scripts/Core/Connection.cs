using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Connection
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;
    private readonly SynchronizationContext syncContext;
    public IPEndPoint RemoteEndPoint => client.Client.RemoteEndPoint as IPEndPoint;
    public IPEndPoint LocalEndPoint => client.Client.LocalEndPoint as IPEndPoint;
    public bool IsServer { get; }
    public DataReader DataReader { get; }
    public StreamWriter StreamWriter { get; }
    public event Action<Connection> DisconnectedEvent;

    public Connection(TcpClient client, bool isServer)
    {
        this.client = client;
        IsServer = isServer;
        syncContext = SynchronizationContext.Current;
        stream = this.client.GetStream();
        DataReader = new DataReader(stream);
        StreamWriter = new StreamWriter(stream);
        DataReader.ReadDisconnectAsyncEvent += OnReadDisconnectDataAsync;
    }

    private void OnReadDisconnectDataAsync(DataReader reader)
    {
        syncContext.Post(_ => Disconnect(), null);
    }

    public void Disconnect()
    {
        DataReader.ReadDisconnectAsyncEvent -= OnReadDisconnectDataAsync;
        stream.Close();
        client.Close();
        DisconnectedEvent?.Invoke(this);
    }
}
public class ConnectionMaker
{

}
