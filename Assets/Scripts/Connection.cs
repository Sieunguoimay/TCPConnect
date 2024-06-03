using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Connection : IDisposable
{
    public Channel Channel { get; private set; }
    public ConnectionManager Manager { get; }

    public Connection(ConnectionManager manager, TcpClient connectedClient, Action<Channel> disconnectCallback)
    {
        if (connectedClient.Connected)
        {
            Channel = new Channel(connectedClient, disconnectCallback);
        }
        else
        {
            Debug.Log("Failed to setup connection. Client is not connected");
        }
        Manager = manager;
    }

    public void Dispose()
    {
        Channel?.Dispose();
        Channel = null;
    }

    public void DestroyConnection()
    {
        Manager.DestroyConnection(this);
    }
}