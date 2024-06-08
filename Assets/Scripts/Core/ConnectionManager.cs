using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


public class ConnectionManager
{
    public Listener Listener { get; } = new();
    public Connector Connector { get; } = new();
    public ConnectionContainer Container { get; } = new();
    public event Action<ConnectionManager, Connection> NewConnectionCreatedEvent;

    public void Setup()
    {
        Listener.NewClientConnected -= OnNewClientProvided_ByListener;
        Listener.NewClientConnected += OnNewClientProvided_ByListener;
        Connector.NewClientConnected -= OnNewClientProvided_ByConnector;
        Connector.NewClientConnected += OnNewClientProvided_ByConnector;
    }

    public void TearDown()
    {
        Connector.NewClientConnected -= OnNewClientProvided_ByConnector;
        Listener.NewClientConnected -= OnNewClientProvided_ByListener;

        foreach (var c in Container.Connections)
        {
            c.DisconnectedEvent -= OnConnectionDisconnected;
            c.Disconnect();
        }

        Container.Clear();
    }

    private void OnNewClientProvided_ByListener(Listener provider, TcpClient client)
    {
        HandleClient(client, true);
    }

    private void OnNewClientProvided_ByConnector(Connector provider, TcpClient client)
    {
        HandleClient(client, false);
    }

    private void HandleClient(TcpClient client, bool isServer)
    {
        var address = (client.Client.RemoteEndPoint as IPEndPoint)?.Address;

        if (Container.Connections.Any(c => c.RemoteEndPoint.Address == address))
        {
            Debug.LogError("This connection already exist. Attempt to disconnect it");

            client.Close();
        }
        else
        {
            var connection = new Connection(client, isServer);
            connection.DisconnectedEvent += OnConnectionDisconnected;
            Container.AddConnection(connection);
            Debug.Log($"Added new connection {address}");
            NewConnectionCreatedEvent?.Invoke(this, connection);
        }
    }

    private void OnConnectionDisconnected(Connection connection)
    {
        Container.RemoveConnection(connection);
    }
}
