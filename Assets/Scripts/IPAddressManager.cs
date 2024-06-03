using System;
using System.Net;
using UnityEngine;

public class IPAddressManager
{
    public static readonly int PORT = 7004;
    // public Listener Listener { get; } = new(new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), PORT));
    // public Connector Connector { get; } = new();

    // public Channel Connection { get; private set; }
    // // public bool IsConnected => Connection != null;
    // public event Action<ConnectionMaker> IsConnectedChanged;

    // public void Begin()
    // {
    //     Listener.TryStartServer();
    //     TearDownEventHandlers();
    //     SetupEventHandlers();
    // }

    // public void End()
    // {
    //     TearDownEventHandlers();
    //     Listener.TryStopServer();
    //     Connector.TryStopClient();
    // }

    // private void OnClientConnectedChanged(Connector client)
    // {
    //     if (client.IsConnected)
    //     {
    //         // Connection = client.Connection;
    //         // ListenToConnection();
    //         TearDownEventHandlers();

    //         // Server.TryStopServer();
    //     }
    //     else
    //     {
    //         TearDownEventHandlers();
    //         SetupEventHandlers();
    //     }
    // }

    // private void OnServerConnectedChanged(Listener server)
    // {
    //     if (server.IsConnected)
    //     {
    //         // Connection = server.Connection;
    //         // ListenToConnection();
    //         TearDownEventHandlers();

    //         // Client.TryStopClient();
    //     }
    //     else
    //     {
    //         TearDownEventHandlers();
    //         SetupEventHandlers();
    //     }
    // }

    // // private void ListenToConnection()
    // // {
    // //     Connection.DisconnectedEvent += OnDisconnected;
    // // }

    // // private void OnDisconnected(Channel connection)
    // // {
    // //     Server.TryStartServer();
    // // }

    // private void SetupEventHandlers()
    // {
    //     Listener.IsConnectedChanged += OnServerConnectedChanged;
    //     Connector.IsConnectedChanged += OnClientConnectedChanged;
    // }

    // private void TearDownEventHandlers()
    // {
    //     Connector.IsConnectedChanged -= OnClientConnectedChanged;
    //     Listener.IsConnectedChanged -= OnServerConnectedChanged;
    // }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var localIP = "127.0.0.1";
        foreach (var ip in host.AddressList)
        {
            if (ip == null) continue;
            Debug.Log($"{ip} {ip.AddressFamily}");
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ip.ToString().StartsWith("192.168"))
            {
                localIP = ip.ToString();
            }
        }
        return localIP;
        // throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
