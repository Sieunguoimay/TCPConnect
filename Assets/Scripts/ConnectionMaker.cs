using System;
using System.Net;

public class ConnectionMaker
{
    public static readonly int PORT = 7004;
    public Server Server { get; } = new(new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), PORT));
    public Client Client { get; } = new();

    public Connection Connection { get; private set; }
    public bool IsConnected => Connection != null;
    public event Action<ConnectionMaker> IsConnectedChanged;

    public void Begin()
    {
        Server.TryStartServer();
        TearDownEventHandlers();
        SetupEventHandlers();
    }

    public void End()
    {
        TearDownEventHandlers();
        Server.TryStopServer();
        Client.TryStopClient();
    }

    private void OnClientConnectedChanged(Client client)
    {
        if (client.IsConnected)
        {
            Connection = client.Connection;
            ListenToConnection();
            TearDownEventHandlers();

            // Server.TryStopServer();
        }
        else
        {
            TearDownEventHandlers();
            SetupEventHandlers();
        }
    }

    private void OnServerConnectedChanged(Server server)
    {
        if (server.IsConnected)
        {
            Connection = server.Connection;
            ListenToConnection();
            TearDownEventHandlers();

            // Client.TryStopClient();
        }
        else
        {
            TearDownEventHandlers();
            SetupEventHandlers();
        }
    }

    private void ListenToConnection()
    {
        Connection.DisconnectedEvent += OnDisconnected;
    }

    private void OnDisconnected(Connection connection)
    {
        Server.TryStartServer();
    }

    private void SetupEventHandlers()
    {
        Server.IsConnectedChanged += OnServerConnectedChanged;
        Client.IsConnectedChanged += OnClientConnectedChanged;
    }

    private void TearDownEventHandlers()
    {
        Client.IsConnectedChanged -= OnClientConnectedChanged;
        Server.IsConnectedChanged -= OnServerConnectedChanged;
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
