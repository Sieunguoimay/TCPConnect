using UnityEngine;

public class ServerMB : MonoBehaviour
{
    [SerializeField] private int port = 1;

    private Server _server;
    private Server Server => _server ??= new(port);

    [ContextMenu("StartServer")]
    public void StartServer() => Server.TryStartServer();

    [ContextMenu("StopServer")]
    public void StopServer() => Server.TryStopServer();
}
