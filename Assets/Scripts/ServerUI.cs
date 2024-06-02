using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class ServerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI connectionStatus;
    [SerializeField] private TextMeshProUGUI runningStatus;
    private Server _server;
    private SynchronizationContext _syncContext;
    public bool IsConnected => _server?.IsConnected ?? false;
    public bool IsStarted => _server?.IsStarted ?? false;

    public void Setup(Server server)
    {
        _syncContext = SynchronizationContext.Current;
        _server = server;

        _server.IsConnectedChanged -= OnServerConnectedChanged;
        _server.IsConnectedChanged += OnServerConnectedChanged;
        _server.IsStartedChanged -= OnServerStartedChanged;
        _server.IsStartedChanged += OnServerStartedChanged;

        UpdateView_ConnectionStatus();
        UpdateView_RunningStatus();
    }

    public void TearDown()
    {
        _server.IsStartedChanged -= OnServerStartedChanged;
        _server.IsConnectedChanged -= OnServerConnectedChanged;
        _server = null;
    }

    private void OnServerStartedChanged(Server server)
    {
        _syncContext.Post(_ => UpdateView_RunningStatus(), null);
    }
    private void OnServerConnectedChanged(Server server)
    {
        Debug.Log($"ServerUI: OnServerConnectedChanged {IsConnected}");
        _syncContext.Post(_ => UpdateView_ConnectionStatus(), null);
    }

    private void UpdateView_ConnectionStatus()
    {
        Debug.Log($"ServerUI: UpdateView_ConnectionStatus {IsConnected}");
        connectionStatus.text = IsConnected ? "Connected" : "Disconnected";
        Debug.Log($"ServerUI: UpdateView_ConnectionStatus {connectionStatus.text}");
    }
    private void UpdateView_RunningStatus()
    {
        if (IsStarted)
        {
            runningStatus.text = $"Server Running at {_server.IPEndpoint.Address}:{_server.IPEndpoint.Port}";
        }
        else
        {
            runningStatus.text = $"Server is Off";
        }
    }


}
