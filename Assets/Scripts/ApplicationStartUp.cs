using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ApplicationStartUp : MonoBehaviour
{
    public ConnectionManager ConnectionManager { get; } = new();
    public Listener Listener { get; } = new();
    public Connector Connector { get; } = new();

    private IEnumerable<IClientProvider> Providers
    {
        get
        {
            yield return Connector;
            yield return Listener;
        }
    }

    private void Start()
    {
        StartUp();
    }

    private void OnDestroy()
    {
        ShutDown();
    }

    private void OnApplicationQuit()
    {
        ShutDown();
    }

    private void StartUp()
    {
        ConnectionManager.Setup(Providers);
        var serverEndpoint = new IPEndPoint(IPAddress.Parse(IPAddressManager.GetLocalIPAddress()), IPAddressManager.PORT);
        Listener.TryStartServer(serverEndpoint);
    }

    private void ShutDown()
    {
        Listener.TryStopServer();
        ConnectionManager.TearDown();
    }
}
