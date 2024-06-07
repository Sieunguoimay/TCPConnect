using System;
using System.Net;
using System.Net.Sockets;

public class Connection
{
    public NetworkStream Stream { get; }
    public TcpClient Client { get; }
    public bool IsServer { get; }
    public IPEndPoint RemoteEndPoint => Client.Client.RemoteEndPoint as IPEndPoint;
    public IPEndPoint LocalEndPoint => Client.Client.LocalEndPoint as IPEndPoint;
    public event Action<Connection> DisconnectedEvent;

    public Connection(TcpClient client, bool isServer)
    {
        Client = client;
        IsServer = isServer;
        Stream = Client.GetStream();
    }

    public void Disconnect()
    {
        Stream.Close();
        Client.Close();
        DisconnectedEvent?.Invoke(this);
    }
}
