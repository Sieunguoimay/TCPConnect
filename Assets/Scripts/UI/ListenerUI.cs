using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class ListenerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI connectionStatus;
    [SerializeField] private TextMeshProUGUI runningStatus;
    private Listener _listener;
    public bool IsStarted => _listener?.IsStarted ?? false;

    public void Setup(Listener server)
    {
        _listener = server;
        _listener.StateChanged -= OnServerStartedChanged;
        _listener.StateChanged += OnServerStartedChanged;

        UpdateView_RunningStatus();
    }

    public void TearDown()
    {
        _listener.StateChanged -= OnServerStartedChanged;
        _listener = null;
    }

    private void OnServerStartedChanged(Listener server)
    {
        UpdateView_RunningStatus();
    }

    private void UpdateView_RunningStatus()
    {
        if (IsStarted)
        {
            runningStatus.text = $"Server Running at {_listener.IPEndpoint.Address}:{_listener.IPEndpoint.Port}";
        }
        else
        {
            runningStatus.text = $"Server is Off";
        }
    }
}
