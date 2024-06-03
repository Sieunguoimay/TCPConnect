using System;
using System.Collections.Generic;

public class ConnectionContainer
{
    private readonly List<Connection> connections = new();
    public IReadOnlyList<Connection> Connections => connections;
    public event Action<ConnectionContainer> ConnectionsListChanged;

    public void AddConnection(Connection connection)
    {
        connections.Add(connection);
        ConnectionsListChanged?.Invoke(this);
    }

    public void RemoveConnection(Connection connection)
    {
        connections.Remove(connection);
        ConnectionsListChanged?.Invoke(this);
    }

    public void Clear()
    {
        connections.Clear();
        ConnectionsListChanged?.Invoke(this);
    }
}
