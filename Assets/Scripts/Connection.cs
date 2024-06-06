using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public interface IConnectionDestroyer
{
    void DestroyConnection(Connection connection);
}

public class Connection : IDisposable
{
    public Channel Channel { get; private set; }
    public bool IsServer { get; }
    private readonly IConnectionDestroyer destroyer;

    public Connection(IConnectionDestroyer destroyer, TcpClient connectedClient, Action<Channel> disconnectCallback, bool isServer)
    {
        this.destroyer = destroyer;
        IsServer = isServer;

        if (connectedClient.Connected)
        {
            Channel = new Channel(connectedClient, disconnectCallback);
        }
        else
        {
            Debug.Log("Failed to setup connection. Client is not connected");
        }
    }

    public void Dispose()
    {
        Channel?.Dispose();
        Channel = null;
    }

    public void DestroyConnection()
    {
        destroyer.DestroyConnection(this);
    }
}