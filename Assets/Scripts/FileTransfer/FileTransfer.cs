using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

public interface IConnectionTool
{
    void Setup(Connection connection);
    void TearDown();
    Connection ControlConnection { get; }
}

public class FileTransfer : IConnectionTool
{
    public Connection ControlConnection { get; private set; }
    private Channel _dataChannel;
    private readonly Listener listener = new();
    // private byte[] _dataToSend;

    public void Setup(Connection connection)
    {
        ControlConnection = connection;
        listener.SetClientConnectedCallback(OnConnectionConnected);
    }

    public void TearDown()
    {
        _dataChannel?.Dispose();
        _dataChannel = null;
    }

    private void OnConnectionConnected(IClientProvider provider, TcpClient client)
    {
        UnityEngine.Debug.Log("FileTransferManager: OnConnectionConnected");

        _dataChannel = new Channel(client, OnDataChannelDisconnected);
        // _dataChannel.Writer.Write(_dataToSend);
        // _dataChannel.Writer.Flush();
    }

    private void OnDataChannelDisconnected(Channel channel)
    {
        _dataChannel = null;
        UnityEngine.Debug.Log("FileTransferManager: OnDataChannelDisconnected");
    }

    public void SendData(byte[] bytes)
    {
        // _dataToSend = bytes;

        listener.TryStartServer(new IPEndPoint(IPAddress.Any, 0));

        var message = $"FILE_TRANSFER {listener.IPEndpoint.Address} {listener.IPEndpoint.Port}";

        ControlConnection.Channel.Writer.Write(message);
        ControlConnection.Channel.Writer.Flush();

        UnityEngine.Debug.Log(message);
    }
}
