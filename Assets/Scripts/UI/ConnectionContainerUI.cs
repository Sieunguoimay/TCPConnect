using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionContainerUI : MonoBehaviour
{
    [SerializeField] private ConnectionUI prefab;
    [SerializeField] private Transform parent;

    private IEnumerable<ConnectionUI> _connectionUIs;
    private ConnectionContainer _container;

    public void Setup(ConnectionContainer container)
    {
        _container = container;
        _container.ConnectionsListChanged -= OnContainerChanged;
        _container.ConnectionsListChanged += OnContainerChanged;
        _connectionUIs = container.Connections.Select(CreateConnectionUI).ToArray();
    }

    public void TearDown()
    {
        foreach (var c in _connectionUIs)
        {
            c.TearDown();
        }
        _connectionUIs = null;
        _container.ConnectionsListChanged -= OnContainerChanged;
    }

    private void OnContainerChanged(ConnectionContainer container)
    {
        foreach (var c in _connectionUIs)
        {
            c.TearDown();
            Destroy(c.gameObject);
        }
        _connectionUIs = container.Connections.Select(CreateConnectionUI).ToArray();
    }

    private ConnectionUI CreateConnectionUI(Connection connection)
    {
        var ui = Instantiate(prefab, parent);
        ui.Setup(connection);
        return ui;
    }
}