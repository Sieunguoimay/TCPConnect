using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server
{
    public IPEndPoint IPEndpoint { get; }
    private TcpListener _listener;
    public Connection Connection { get; private set; }

    public bool IsConnected => Connection != null;
    public bool IsStarted => _listener != null;
    public event Action<Server> IsStartedChanged;
    public event Action<Server> IsConnectedChanged;
    private SynchronizationContext _syncContext;

    public Server(int port)
    {
        IPEndpoint = new(Dns.GetHostEntry("localhost").AddressList[1], port);
    }

    public Server(IPEndPoint endPoint)
    {
        IPEndpoint = endPoint;
    }

    [ContextMenu("StartServer")]
    public void TryStartServer()
    {
        if (_listener == null)
        {
            _listener = new TcpListener(IPEndpoint);
            _listener.Start();
            BeginAcceptClient();
            Debug.Log($"StartServer at {IPEndpoint.Address} {IPEndpoint.Port} {string.Join(",", Dns.GetHostEntry("localhost").AddressList.Select(s => s))}");
            IsStartedChanged?.Invoke(this);
        }
        else
        {
            Debug.LogError("Server already started");
        }
    }

    public void TryStopServer()
    {
        Connection?.Disconnect();
        if (_listener != null)
        {
            Debug.Log("StopServer");
            _listener.Stop();
            _listener = null;
            IsStartedChanged?.Invoke(this);
        }
    }

    [ContextMenu("BeginAcceptClient")]
    public void BeginAcceptClient()
    {
        _syncContext = SynchronizationContext.Current;
        _listener?.BeginAcceptTcpClient(OnAcceptTcpClient, null);
    }

    private void OnAcceptTcpClient(IAsyncResult result)
    {
        var client = _listener.EndAcceptTcpClient(result);
        BeginAcceptClient();
        _syncContext.Post(_ => TryCreateConnection(client), null);
    }

    private void TryCreateConnection(TcpClient client)
    {
        Debug.Log($"OnAcceptTcpClient {client}");

        if (Connection == null)
        {
            Connection = new Connection(client);
            Connection.DisconnectedEvent += OnConnectionDisconnected;
            Connection.ReceivedDataEvent += OnReceivedData;
            IsConnectedChanged?.Invoke(this);
        }
        else
        {
            client.Close();
            Debug.Log("Server:Client refused. Already connected to another client");
        }
    }

    private void OnReceivedData(Connection connection, byte[] data)
    {
        Debug.Log(Encoding.UTF8.GetString(data));
    }

    private void OnConnectionDisconnected(Connection connection)
    {
        Connection.DisconnectedEvent -= OnConnectionDisconnected;
        Connection.ReceivedDataEvent -= OnReceivedData;
        Connection.Client.Close();
        Connection = null;
        IsConnectedChanged?.Invoke(this);
    }

    [ContextMenu("Ping client")]
    private void PingClient()
    {
        Connection?.Writer?.WriteLine("What'sup brother. Iam server");
        Connection?.Writer?.Flush();
    }
}
