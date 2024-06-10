using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

public class FileTransferTool
{
    private Dependencies _dependencies;
    private MessageReader _controlMessageReader;
    private readonly ConnectionManager dataConnectionManager = new();
    public Connection DataConnection { get; private set; }
    public event Action<FileTransferTool> ConnectionStatusChangedEvent;
    private DataConnectionAction _dataConnectionAction = DataConnectionAction.NONE;

    private string _filePathToRead;
    private string _fileNameToWrite;

    public void Setup(Dependencies dependencies)
    {
        _dependencies = dependencies;
        dataConnectionManager.NewConnectionCreatedEvent -= OnNewConenctionCreated;
        dataConnectionManager.NewConnectionCreatedEvent += OnNewConenctionCreated;
        _controlMessageReader = new MessageReader(_dependencies.ControlConnection.DataReader, OnReceiveControlMessage);
    }

    public void TearDown()
    {
        _controlMessageReader.Dispose();
        dataConnectionManager.NewConnectionCreatedEvent -= OnNewConenctionCreated;
    }

    public void StartMakingDataConnection(string filePath, Dictionary<string, string> metadata)
    {
        _filePathToRead = filePath;
        if (_dependencies.ControlConnection.IsServer)
        {
            dataConnectionManager.Listener.TryStartServer(new IPEndPoint(_dependencies.ControlConnection.LocalEndPoint.Address, 0));
            metadata["port"] = dataConnectionManager.Listener.LocalEndPoint.Port.ToString();
            metadata["ipaddress"] = dataConnectionManager.Listener.LocalEndPoint.Address.ToString();

            var extraDataStr = DictionaryToString(metadata);
            var cmd = "CONNECT_AND_RECEIVE_FILE";
            _dependencies.ControlConnection.StreamWriter.WriteLine($"{cmd}?{extraDataStr}");
            _dependencies.ControlConnection.StreamWriter.Flush();

            _dataConnectionAction = DataConnectionAction.SEND_FILE;

            Logger.Log($"StartMakingDataConnection CONNECT_AND_RECEIVE_FILE");

        }
        else
        {
            var extraDataStr = DictionaryToString(metadata);
            var cmd = "LISTEN_AND_RECEIVE_FILE";
            _dependencies.ControlConnection.StreamWriter.WriteLine($"{cmd}?{extraDataStr}");
            _dependencies.ControlConnection.StreamWriter.Flush();

            Logger.Log($"StartMakingDataConnection LISTEN_AND_RECEIVE_FILE");
        }
    }

    private void OnReceiveControlMessage(string msg)
    {
        Logger.Log($"OnReceiveControlMessage msg={msg}");

        var splitted = msg.Split("?");
        if (splitted.Length <= 1)
        {
            return;
        }
        var receivedCmd = splitted[0];
        var metadata = StringToDictionary(splitted[1]);

        if (receivedCmd == "CONNECT_AND_RECEIVE_FILE")
        {
            try
            {
                var ipAddress = metadata["ipaddress"];
                var port = int.Parse(metadata["port"]);
                dataConnectionManager.Connector.TryConnect(ipAddress, port);

                _dataConnectionAction = DataConnectionAction.RECEIVE_FILE;
                _fileNameToWrite = metadata["file_name"];
            }
            catch (Exception e)
            {
                Logger.Log($"{e.Message}");
            }
        }
        else
        if (receivedCmd == "CONNECT_AND_SEND_FILE")
        {
            try
            {
                var ipAddress = metadata["ipaddress"];
                var port = int.Parse(metadata["port"]);
                dataConnectionManager.Connector.TryConnect(ipAddress, port);

                _dataConnectionAction = DataConnectionAction.SEND_FILE;

            }
            catch (Exception e)
            {
                Logger.Log($"{e.Message}");
            }
        }
        else
        if (receivedCmd == "LISTEN_AND_RECEIVE_FILE")
        {
            try
            {
                var sendingMetadata = new Dictionary<string, string>();

                dataConnectionManager.Listener.TryStartServer(new IPEndPoint(_dependencies.ControlConnection.LocalEndPoint.Address, 0));
                sendingMetadata["ipaddress"] = dataConnectionManager.Listener.LocalEndPoint.Address.ToString();
                sendingMetadata["port"] = dataConnectionManager.Listener.LocalEndPoint.Port.ToString();

                var extraDataStr = DictionaryToString(sendingMetadata);
                var cmd = "CONNECT_AND_SEND_FILE";
                _dependencies.ControlConnection.StreamWriter.WriteLine($"{cmd}?{extraDataStr}");
                _dependencies.ControlConnection.StreamWriter.Flush();

                _dataConnectionAction = DataConnectionAction.RECEIVE_FILE;
                _fileNameToWrite = metadata["file_name"];

                Logger.Log($"Flush {cmd}?{extraDataStr}");
            }
            catch (Exception e)
            {
                Logger.Log($"{e.Message}");
            }
        }
    }

    private void OnNewConenctionCreated(ConnectionManager manager, Connection connection)
    {
        Logger.Log($"OnNewConenctionCreated");
        if (DataConnection != null)
        {
            Logger.Log($"OnNewConenctionCreated1");
            DataConnection = connection;
            ConnectionStatusChangedEvent?.Invoke(this);

            Logger.Log($"OnNewConenctionCreated2 {_dataConnectionAction}");
            if (_dataConnectionAction == DataConnectionAction.RECEIVE_FILE)
            {
                HandleFileReceiving();
            }
            else
            if (_dataConnectionAction == DataConnectionAction.SEND_FILE)
            {
                DoFileSending();
            }
        }
    }

    private void DoFileSending()
    {
        DataConnection.StreamWriter.Write($"HERE THE FILE {_filePathToRead}");

        UnityEngine.Debug.Log($"SENT: HERE THE FILE {_filePathToRead}");
        Logger.Log($"SENT: HERE THE FILE {_filePathToRead}");
        // DataConnection.Disconnect();
        // DataConnection = null;
        // ConnectionStatusChangedEvent?.Invoke(this);
    }

    private void HandleFileReceiving()
    {
        // new ReadAndSaveToFile(DataConnection.DataReader, _fileNameToWrite, _ => _.Dispose());
        new MessageReader(DataConnection.DataReader, msg =>
        {
            UnityEngine.Debug.Log($"RECEIVED: {msg} about to store at {_fileNameToWrite}");
            Logger.Log($"RECEIVED: {msg} about to store at {_fileNameToWrite}");
        });
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
    private enum DataConnectionAction
    {
        NONE = 0,
        SEND_FILE = 1,
        RECEIVE_FILE = 2,
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
        Logger.Log($"OnReadDataAsync {str}");
        stringBuilder.Append(str);
        var message = stringBuilder.ToString();
        if (message.EndsWith("\n"))
        {
            onReceivedCallback?.Invoke(message);
        }
    }
}

public class ReadAndSaveToFile : IDisposable
{
    private readonly DataReader dataReader;
    private readonly string savePath;
    private readonly Action<ReadAndSaveToFile> doneCallback;

    public ReadAndSaveToFile(DataReader dataReader, string savePath, Action<ReadAndSaveToFile> doneCallback)
    {
        this.dataReader = dataReader;
        this.savePath = savePath;
        this.doneCallback = doneCallback;
        dataReader.ReadDataAsyncEvent += OnReadDataAsync;
    }

    private void OnReadDataAsync(DataReader reader, byte[] arg2, int arg3)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        dataReader.ReadDataAsyncEvent -= OnReadDataAsync;
    }
}