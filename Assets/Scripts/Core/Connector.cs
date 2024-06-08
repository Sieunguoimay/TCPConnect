using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class Connector
{
    private TcpClient _client;
    private readonly SynchronizationContext syncContext;
    public bool IsConnecting => _client != null;
    public event Action<Connector> ConnectStatusChanged;
    public event Action<Connector, TcpClient> NewClientConnected;
    
    public Connector()
    {
        syncContext = SynchronizationContext.Current;
    }

    public void TryConnect(string address, int port)
        => TryConnect(new IPEndPoint(IPAddress.Parse(address), port));

    public void TryConnect(IPEndPoint endPoint)
    {
        if (_client == null)
        {
            _client = new TcpClient();
            _client.BeginConnect(endPoint.Address, endPoint.Port, OnConnected, null);
            Debug.Log($"Client Attempt to connect to {endPoint.Address} {endPoint.Port}");
            ConnectStatusChanged?.Invoke(this);
        }
        else
        {
            Debug.Log($"Waiting for client {(_client.Client.RemoteEndPoint as IPEndPoint)?.Address}");
        }
    }

    private void OnConnected(IAsyncResult result)
    {
        Debug.Log($"Connector: OnConnected client {(_client.Client.RemoteEndPoint as IPEndPoint)?.Address}");
        try
        {
            _client.EndConnect(result);
            var client = _client;
            syncContext.Post(_ => NewClientConnected?.Invoke(this, client), null);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            _client.Close();
        }
        finally
        {
            _client = null;
            syncContext.Post(_ => ConnectStatusChanged?.Invoke(this), null);
        }
    }
}
