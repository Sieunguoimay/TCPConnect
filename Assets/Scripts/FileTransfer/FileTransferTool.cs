using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

public class FileTransferTool
{
    public void Setup()
    {

    }

    public void TearDown()
    {

    }

    public void Transfer(byte[] data, int length)
    {

    }
}

public class DataConnectionMaker
{
    private Dependencies _dependencies;
    private MessageReader _messageReader;
    private readonly ConnectionManager connectionManager = new();
    public Connection DataConnection { get; private set; }
    public event Action<DataConnectionMaker> ConnectionReadyEvent;

    public void Setup(Dependencies dependencies)
    {
        _dependencies = dependencies;
        connectionManager.NewConnectionCreatedEvent -= OnNewConenctionCreated;
        connectionManager.NewConnectionCreatedEvent += OnNewConenctionCreated;
        _messageReader = new MessageReader(_dependencies.ControlConnection.DataReader, OnReceiveControlMessage);
    }

    public void TearDown()
    {
        _messageReader.Dispose();
        connectionManager.NewConnectionCreatedEvent -= OnNewConenctionCreated;
    }
    public void StartMakingDataConnection(Dictionary<string, string> extraData)
    {
        if (_dependencies.ControlConnection.IsServer)
        {
            var endpoint = new IPEndPoint(_dependencies.ControlConnection.LocalEndPoint.Address, 0);
            connectionManager.Listener.TryStartServer(endpoint);

            extraData["port"] = endpoint.Port.ToString();
            extraData["ipaddress"] = endpoint.Address.ToString();

            var extraDataStr = DictionaryToString(extraData);

            _dependencies.ControlConnection.StreamWriter.WriteLine($"CONNECT_AND_RECEIVE_FILE?{extraDataStr}");
            _dependencies.ControlConnection.StreamWriter.Flush();
        }
        else
        {
            var extraDataStr = DictionaryToString(extraData);
            _dependencies.ControlConnection.StreamWriter.WriteLine($"LISTEN_AND_RECEIVE_FILE?{extraDataStr}");
            _dependencies.ControlConnection.StreamWriter.Flush();
        }
    }

    private void OnReceiveControlMessage(string msg)
    {
        var splitted = msg.Split("?");
        var cmd = splitted[0];
        var dic = StringToDictionary(splitted[1]);

        if (cmd == "CONNECT_AND_RECEIVE_FILE")
        {
            var ipAddress = dic["ipaddress"];
            var port = int.Parse(dic["port"]);
            connectionManager.Connector.TryConnect(ipAddress, port);
        }
        else
        if (cmd == "LISTEN_AND_RECEIVE_FILE")
        {
            var endpoint = new IPEndPoint(_dependencies.ControlConnection.LocalEndPoint.Address, 0);
            connectionManager.Listener.TryStartServer(endpoint);

            var extraData = new Dictionary<string, string>();

            extraData["port"] = endpoint.Port.ToString();
            extraData["ipaddress"] = endpoint.Address.ToString();

            var extraDataStr = DictionaryToString(extraData);

            _dependencies.ControlConnection.StreamWriter.WriteLine($"CONNECT_AND_SEND_FILE?{extraDataStr}");
            _dependencies.ControlConnection.StreamWriter.Flush();
        }
    }

    private void OnNewConenctionCreated(ConnectionManager manager, Connection connection)
    {
        if (DataConnection != null)
        {
            DataConnection = connection;
            ConnectionReadyEvent?.Invoke(this);
        }
    }

    public static string DictionaryToString(Dictionary<string, string> data)
    {
        return string.Join("&", data.Select(i => $"{i.Key}={i.Value}"));
    }

    public static Dictionary<string, string> StringToDictionary(string data)
    {
        var dic = new Dictionary<string, string>();
        var splitedPairs = data.Split("&");
        foreach (var p in splitedPairs)
        {
            var parts = p.Split("=");
            if (parts.Length == 2)
            {
                dic[parts[0]] = parts[1];
            }
        }
        return dic;
    }

    public class Dependencies
    {
        public Connection ControlConnection { get; }

        public Dependencies(Connection controlConnection)
        {
            ControlConnection = controlConnection;
        }
    }
}

public class MessageReader : IDisposable
{
    private readonly DataReader dataReader;
    private readonly Action<string> onReceivedCallback;
    private readonly StringBuilder stringBuilder = new();

    public MessageReader(DataReader dataReader, Action<string> onReceivedCallback)
    {
        this.dataReader = dataReader;
        this.onReceivedCallback = onReceivedCallback;
        dataReader.ReadDataAsyncEvent += OnReadDataAsync;
    }

    public void Dispose()
    {
        dataReader.ReadDataAsyncEvent -= OnReadDataAsync;
    }

    private void OnReadDataAsync(DataReader reader, byte[] arg2, int arg3)
    {
        var str = Encoding.ASCII.GetString(arg2, 0, arg3);
        stringBuilder.Append(str);
        var message = stringBuilder.ToString();
        if (message.EndsWith("\n"))
        {
            onReceivedCallback?.Invoke(message);
        }
    }
}

