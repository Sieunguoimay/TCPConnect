using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ApplicationStartUp : MonoBehaviour
{
    public ConnectionManager ControlConnectionManager { get; } = new();

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
        ControlConnectionManager.Setup();
        ControlConnectionManager.Listener.TryStartServer(IPAddressManager.GetLocalIPEndPoint());
    }

    private void ShutDown()
    {
        ControlConnectionManager.Listener.TryStopServer();
        ControlConnectionManager.TearDown();
    }
}
