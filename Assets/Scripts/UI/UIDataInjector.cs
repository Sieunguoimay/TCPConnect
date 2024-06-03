using System;
using UnityEngine;

public class UIDataInjector : MonoBehaviour
{
    [SerializeField] private ConnectorUI connector;
    [SerializeField] private ListenerUI listener;
    [SerializeField] private ConnectionContainerUI connectionContainer;
    [SerializeField] private ApplicationStartUp startup;

    private void OnEnable()
    {
        connector.Setup(startup.Connector);
        listener.Setup(startup.Listener);
        connectionContainer.Setup(startup.ConnectionManager.Container);
    }

    private void OnDisable()
    {
        connector.TearDown();
        listener.TearDown();
        connectionContainer.TearDown();
    }
}
