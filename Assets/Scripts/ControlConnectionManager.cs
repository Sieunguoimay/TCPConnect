using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public interface IClientProvider
{
    void SetClientConnectedCallback(Action<IClientProvider, TcpClient> clientConnectedCallback);
}

public class ControlConnectionManager
{
    private IEnumerable<IClientProvider> _providers;
    public ConnectionContainer Container { get; } = new();

    public void Setup(IEnumerable<IClientProvider> providers)
    {
        _providers = providers;
        foreach (var c in _providers)
        {
            c.SetClientConnectedCallback(OnNewClientProvided);
        }
    }

    public void TearDown()
    {
        foreach (var c in _providers)
        {
            c.SetClientConnectedCallback(null);
        }

        foreach (var c in Container.Connections)
        {
            c.DisconnectedEvent -= OnConnectionDisconnected;
            c.Disconnect();
        }

        Container.Clear();
    }

    private void OnNewClientProvided(IClientProvider provider, TcpClient client)
    {
        var address = (client.Client.RemoteEndPoint as IPEndPoint)?.Address;

        if (Container.Connections.Any(c => (c.Client.Client.RemoteEndPoint as IPEndPoint)?.Address == address))
        {
            Debug.LogError("This connection already exist. Attempt to disconnect it");

            client.Close();
        }
        else
        {
            var connection = new Connection(client, provider is Listener);
            connection.DisconnectedEvent += OnConnectionDisconnected;
            Container.AddConnection(connection);
            Debug.Log($"Added new connection {address}");
        }
    }

    private void OnConnectionDisconnected(Connection connection)
    {
        Container.RemoveConnection(connection);
    }
}
