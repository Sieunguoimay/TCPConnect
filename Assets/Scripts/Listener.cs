using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Listener : IClientProvider
{
    public IPEndPoint IPEndpoint { get; private set; }
    private SynchronizationContext _syncContext;
    private TcpListener _listener;
    public bool IsStarted => _listener != null;
    public event Action<Listener> StateChanged;
    private Action<IClientProvider, TcpClient> _newConnectedClientCallback;

    public void SetClientConnectedCallback(Action<IClientProvider, TcpClient> clientConnectedCallback)
    {
        _newConnectedClientCallback = clientConnectedCallback;
    }

    public void TryStartServer(int port)
        => TryStartServer(new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[1], port));

    public void TryStartServer(IPEndPoint endPoint)
    {
        if (_listener == null)
        {
            IPEndpoint = endPoint;
            _listener = new TcpListener(IPEndpoint);
            _syncContext = SynchronizationContext.Current;
            _listener.Start();
            Debug.Log($"StartServer at {IPEndpoint.Address} {IPEndpoint.Port} {string.Join(",", Dns.GetHostEntry("localhost").AddressList.Select(s => s))}");
            BeginAcceptClient();
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

    private void BeginAcceptClient()
    {
        if (_listener == null) return;
        Debug.Log("BeginAcceptClient");
        _listener?.BeginAcceptTcpClient(OnAcceptTcpClient, null);
    }

    private void OnAcceptTcpClient(IAsyncResult result)
    {
        try
        {
            if (_listener == null) return;
            Debug.Log("OnAcceptTcpClient");
            var client = _listener.EndAcceptTcpClient(result);
            Debug.Log($"OnAcceptTcpClient2 {client}");
            _syncContext.Post(_ => _newConnectedClientCallback?.Invoke(this, client), null);
            Debug.Log($"OnAcceptTcpClient3");
            BeginAcceptClient();
            Debug.Log($"OnAcceptTcpClient4");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    // private void TryCreateConnection(TcpClient client)
    // {
    //     Debug.Log($"TryCreateConnection {client}");

    //     if (Connection == null)
    //     {
    //         Connection = new Channel(client);
    //         Connection.DisconnectedEvent += OnConnectionDisconnected;
    //         Connection.ReceivedDataEvent += OnReceivedData;
    //         IsConnectedChanged?.Invoke(this);
    //     }
    //     else
    //     {
    //         client.Close();
    //         Debug.Log("Server:Client refused. Already connected to another client");
    //     }
    // }

    // private void OnReceivedData(Channel connection, byte[] data)
    // {
    //     Debug.Log(Encoding.UTF8.GetString(data));
    // }

    // private void OnConnectionDisconnected(Channel connection)
    // {
    //     Debug.Log("OnConnectionDisconnected");

    //     Connection.DisconnectedEvent -= OnConnectionDisconnected;
    //     Connection.ReceivedDataEvent -= OnReceivedData;
    //     Connection.Client.Close();
    //     Connection = null;
    //     IsConnectedChanged?.Invoke(this);
    // }

    // [ContextMenu("Ping client")]
    // private void PingClient()
    // {
    //     Connection?.Writer?.WriteLine("What'sup brother. Iam server");
    //     Connection?.Writer?.Flush();
    // }
}
