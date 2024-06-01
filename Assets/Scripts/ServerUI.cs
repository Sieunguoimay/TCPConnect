using System;
using TMPro;
using UnityEngine;

public class ServerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI connectionStatus;
    [SerializeField] private TextMeshProUGUI runningStatus;
    private Server _server;
    public bool IsConnected => _server?.IsConnected ?? false;
    public bool IsStarted => _server?.IsStarted ?? false;

    public void Setup(Server server)
    {
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
        UpdateView_RunningStatus();
    }
    private void OnServerConnectedChanged(Server server)
    {
        UpdateView_ConnectionStatus();
    }

    private void UpdateView_ConnectionStatus()
    {
        connectionStatus.text = IsConnected ? "Connected" : "Disconnected";
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
