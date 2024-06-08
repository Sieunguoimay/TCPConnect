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
        connector.Setup(startup.ControlConnectionManager.Connector);
        listener.Setup(startup.ControlConnectionManager.Listener);
        connectionContainer.Setup(startup.ControlConnectionManager.Container);
    }

    private void OnDisable()
    {
        connector.TearDown();
        listener.TearDown();
        connectionContainer.TearDown();
    }
}
