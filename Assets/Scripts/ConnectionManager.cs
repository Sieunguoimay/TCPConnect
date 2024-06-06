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

public class ConnectionManager : IConnectionDestroyer
{
    private IEnumerable<IClientProvider> Providers;
    public ConnectionContainer Container { get; } = new();

    public void Setup(IEnumerable<IClientProvider> providers)
    {
        Providers = providers;
        foreach (var c in Providers)
        {
            c.SetClientConnectedCallback(OnNewClientProvided);
        }
    }

    public void TearDown()
    {
        foreach (var c in Providers)
        {
            c.SetClientConnectedCallback(null);
        }

        foreach (var c in Container.Connections)
        {
            c.Dispose();
        }
        Container.Clear();
    }

    private void OnNewClientProvided(IClientProvider provider, TcpClient client)
    {
        var address = (client.Client.RemoteEndPoint as IPEndPoint)?.Address;

        if (Container.Connections.Any(c => (c.Channel.Client.Client.RemoteEndPoint as IPEndPoint)?.Address == address))
        {
            Debug.LogError("This connection already exist. Attempt to disconnect it");

            client.Close();
        }
        else
        {
            Container.AddConnection(new Connection(this, client, OnConnectionDisconnectCallback, provider is Listener));
            Debug.Log($"Added new connection {address}");
        }
    }

    private void OnConnectionDisconnectCallback(Channel channel)
    {
        var found = Container.Connections.FirstOrDefault(c => c.Channel == channel);
        if (found != null)
        {
            DestroyConnection(found);
        }
    }

    public void DestroyConnection(Connection connection)
    {
        Container.RemoveConnection(connection);
        connection.Channel.Client.Close();
        connection.Dispose();
    }
}
