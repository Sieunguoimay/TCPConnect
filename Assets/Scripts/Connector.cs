using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class Connector : IClientProvider
{
    private TcpClient _client;
    private SynchronizationContext _syncContext;
    public bool IsConnecting => _client != null;
    public event Action<Connector> ConnectStatusChanged;
    private Action<IClientProvider, TcpClient> _newConnectedClientCallback;

    public void SetClientConnectedCallback(Action<IClientProvider, TcpClient> provideCallback)
    {
        _newConnectedClientCallback = provideCallback;
    }

    public void TryConnect(string address, int port)
        => TryConnect(new IPEndPoint(IPAddress.Parse(address), port));

    public void TryConnect(IPEndPoint endPoint)
    {
        if (_client == null)
        {
            _syncContext = SynchronizationContext.Current;
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
            _syncContext.Post(_ => _newConnectedClientCallback?.Invoke(this, client), null);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            _client.Close();
        }
        finally
        {
            _client = null;
            _syncContext.Post(_ => ConnectStatusChanged?.Invoke(this), null);
        }
    }
}
