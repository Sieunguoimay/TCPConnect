using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Listener
{
    private readonly SynchronizationContext syncContext;
    private TcpListener _listener;
    public bool IsStarted => _listener != null;
    public event Action<Listener> StateChanged;
    public event Action<Listener, TcpClient> NewClientConnected;

    public Listener()
    {
        syncContext = SynchronizationContext.Current;
    }

    public void TryStartServer(int port)
    {
        TryStartServer(new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[1], port));
    }

    public void TryStartServer(IPEndPoint endPoint)
    {
        if (_listener == null)
        {
            _listener = new TcpListener(endPoint);
            _listener.Start();
            _listener.BeginAcceptTcpClient(OnAcceptTcpClient, null);
            Debug.Log($"StartServer at {endPoint.Address} {endPoint.Port} {string.Join(",", Dns.GetHostEntry("localhost").AddressList.Select(s => s))}");
            StateChanged?.Invoke(this);
        }
        else
        {
            Debug.Log("Server already started");
        }
    }

    public void TryStopServer()
    {
        if (_listener != null)
        {
            Debug.Log("StopServer");
            _listener.Stop();
            _listener = null;
            StateChanged?.Invoke(this);
        }
    }

    private void OnAcceptTcpClient(IAsyncResult result)
    {
        try
        {
            if (_listener == null) return;
            Debug.Log("OnAcceptTcpClient");
            var client = _listener.EndAcceptTcpClient(result);
            Debug.Log($"OnAcceptTcpClient2 {client}");
            syncContext.Post(_ => NewClientConnected?.Invoke(this, client), null);
            Debug.Log($"OnAcceptTcpClient3");
            _listener?.BeginAcceptTcpClient(OnAcceptTcpClient, null);
            Debug.Log($"OnAcceptTcpClient4");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
