using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Listener
{
    private SynchronizationContext _syncContext;
    private TcpListener _listener;
    public bool IsStarted => _listener != null;
    public IPEndPoint LocalEndPoint => _listener != null ? (_listener.LocalEndpoint as IPEndPoint) : null;
    public event Action<Listener> StateChanged;
    public event Action<Listener, TcpClient> NewClientConnected;

    public void TryStartServer(int port)
    {
        TryStartServer(new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[1], port));
    }

    public void TryStartServer(IPEndPoint endPoint)
    {
        if (_listener == null)
        {
            try
            {
                _syncContext = SynchronizationContext.Current;
                _listener = new TcpListener(endPoint);
                _listener.Start();
                _listener.BeginAcceptTcpClient(OnAcceptTcpClient, null);
                Debug.Log($"StartServer at {LocalEndPoint.Address} {LocalEndPoint.Port} {string.Join(",", Dns.GetHostEntry("localhost").AddressList.Select(s => s))}");
                Logger.Log($"StartServer at {LocalEndPoint.Address} {LocalEndPoint.Port} {string.Join(",", Dns.GetHostEntry("localhost").AddressList.Select(s => s))}");
                StateChanged?.Invoke(this);

            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
            }
        }
        else
        {
            Debug.Log("Server already started");
            Logger.Log("Server already started");
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
            _syncContext.Post(_ => NewClientConnected?.Invoke(this, client), null);
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
