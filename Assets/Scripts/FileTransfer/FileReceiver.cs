using System;
using System.Diagnostics;
using System.Text;
[Obsolete]
public class FileReceiver : IConnectionTool
{
    private Channel _dataChannel;
    private readonly Connector dataConnector = new();
    public Connection ControlConnection { get; private set; }

    private readonly StringBuilder messageBuilder = new();

    public void Setup(Connection connection)
    {
        ControlConnection = connection;
        // dataConnector.SetClientConnectedCallback(OnDataConnectionConnected);
        ControlConnection.Channel.ReceivedDataEvent -= OnReceivedControlMessage;
        ControlConnection.Channel.ReceivedDataEvent += OnReceivedControlMessage;
    }

    public void TearDown()
    {
        ControlConnection.Channel.ReceivedDataEvent -= OnReceivedControlMessage;
        _dataChannel?.Dispose();
        _dataChannel = null;
    }

    private void OnReceivedControlMessage(Channel channel, byte[] bytes)
    {
        messageBuilder.Append(Encoding.ASCII.GetString(bytes));
        var completeMessage = messageBuilder.ToString();
        if (completeMessage.Contains("\n"))
        {
            UnityEngine.Debug.Log($"Received completed message: {completeMessage}");
        }
    }

    // private void OnDataConnectionConnected(IClientProvider provider, TcpClient client)
    // {
    //     UnityEngine.Debug.Log("FileReceiver: OnDataConnectionConnected");

    //     _dataChannel = new Channel(client, OnDataChannelDisconnected);
    //     // _dataChannel.Writer.Write(_dataToSend);
    //     // _dataChannel.Writer.Flush();
    // }

    // private void OnDataChannelDisconnected(Channel channel)
    // {
    //     _dataChannel = null;
    //     UnityEngine.Debug.Log("FileTransferManager: OnDataChannelDisconnected");
    // }
}