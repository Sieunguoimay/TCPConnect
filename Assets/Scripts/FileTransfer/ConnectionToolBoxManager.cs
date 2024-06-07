using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ConnectionToolBoxManager : MonoBehaviour
{
    [SerializeField] private ApplicationStartUp startUp;
    [SerializeField] private TextMeshProUGUI log;

    private readonly Dictionary<Connection, ConnectionToolBox> toolBoxes = new();
    private ConnectionContainer Container => startUp.ControlConnectionManager.Container;

    private void OnEnable()
    {
        Container.ConnectionsListChanged -= OnConnectionsListChanged;
        Container.ConnectionsListChanged += OnConnectionsListChanged;
    }

    private void OnDisable()
    {
        Container.ConnectionsListChanged -= OnConnectionsListChanged;
    }

    private void OnConnectionsListChanged(ConnectionContainer container)
    {
        foreach (var c in container.Connections)
        {
            if (!toolBoxes.ContainsKey(c))
            {
                toolBoxes.Add(c, new ConnectionToolBox(c));
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
        log.text = $"toolBoxes {toolBoxes.Count}";
    }
}
