using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public class Client
{
    public IPEndPoint IPEndPoint { get; private set; }
    private TcpClient _client;
    public Connection Connection { get; private set; }

    public bool IsConnected => Connection != null;
    public bool IsStarted => _client != null;
    public event Action<Client> IsStartedChanged;
    public event Action<Client> IsConnectedChanged;

    private SynchronizationContext _syncContext;

    public void TryStartClient(string serverAddress, int port)
    {
        if (_client == null)
        {
            IPEndPoint = new(IPAddress.Parse(serverAddress), port);
            _client = new TcpClient();
            _syncContext = SynchronizationContext.Current;
            _client.BeginConnect(IPEndPoint.Address, IPEndPoint.Port, OnConnected, null);
            Debug.Log($"StartClient Connected to {IPEndPoint.Address} {IPEndPoint.Port}");
            IsStartedChanged?.Invoke(this);
        }
        else
        {
            Debug.LogError("Client: Aready started");
        }
    }

    public void TryStopClient()
    {
        if (Connection != null)
        {
            Connection.Disconnect();
        }
        else
        {
            TryCloseClient();
        }
    }

    private void TryCloseClient()
    {
        if (_client != null)
        {
            _client.Close();
            _client = null;
            IsStartedChanged?.Invoke(this);
            Debug.Log($"StopClient");
        }
    }

    private void OnConnected(IAsyncResult result)
    {
        Debug.Log($"Client: OnConnectedToServer");
        try
        {
            _client.EndConnect(result);
            _syncContext.Post(_ => CreateConnection(), null);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            TryCloseClient();
        }
    }

    private void CreateConnection()
    {
        Connection = new Connection(_client);
        Connection.DisconnectedEvent += OnConnectionDisconnected;
        Connection.ReceivedDataEvent += OnReceivedData;
        IsConnectedChanged?.Invoke(this);
    }

    private void OnReceivedData(Connection connection, byte[] data)
    {
        Debug.Log(Encoding.UTF8.GetString(data));
    }

    private void OnConnectionDisconnected(Connection connection)
    {
        Connection.DisconnectedEvent -= OnConnectionDisconnected;
        Connection.ReceivedDataEvent -= OnReceivedData;
        Connection = null;
        IsConnectedChanged?.Invoke(this);

        TryCloseClient();
    }


    [ContextMenu("Ping server")]
    private void PingServer()
    {
        Connection?.Writer?.WriteLine("Hey yo brother. Iam client");
        Connection?.Writer?.Flush();
    }
}
