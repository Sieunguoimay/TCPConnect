using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ConnectionToolBoxManager : MonoBehaviour
{
    private readonly Dictionary<Connection, ConnectionToolBox> toolBoxes = new();
    private ConnectionContainer _container;

    public void Setup(ConnectionContainer container)
    {
        _container = container;
        _container.ConnectionsListChanged -= OnConnectionsListChanged;
        _container.ConnectionsListChanged += OnConnectionsListChanged;
    }

    public void TearDown()
    {
        _container.ConnectionsListChanged -= OnConnectionsListChanged;
        foreach (var b in toolBoxes)
        {
            b.Value.TearDownTools();
        }
    }
    public bool TryGetToolBox(Connection connection, out ConnectionToolBox toolBox)
    {
        return toolBoxes.TryGetValue(connection, out toolBox);
    }

    private void OnConnectionsListChanged(ConnectionContainer container)
    {
        foreach (var c in container.Connections)
        {
            if (!toolBoxes.ContainsKey(c))
            {
                var toolBox = new ConnectionToolBox();
                toolBox.SetupTools(c);
                toolBoxes.Add(c, toolBox);
            }
        }
        while (true)
        {
            var removed = false;
            foreach (var b in toolBoxes)
            {
                if (!container.Connections.Contains(b.Key))
                {
                    b.Value.Dispose();
                    toolBoxes[b.Key].TearDownTools();
                    toolBoxes.Remove(b.Key);
                    removed = true;
                    break;
                }
            }
            if (!removed)
            {
                break;
            }
        }
        Debug.Log($"toolBoxes {toolBoxes.Count}");
        Logger.Log($"toolBoxes {toolBoxes.Count}");
    }
}
