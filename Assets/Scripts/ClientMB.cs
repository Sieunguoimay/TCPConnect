using UnityEngine;

public class ClientMB : MonoBehaviour
{
    [SerializeField] private string serverAddress;
    [SerializeField] private int port;

    private Client _client;
    private Client Client => _client ??= new();

    [ContextMenu("StartClient")]
    public void StartClient() => Client.TryStartClient(serverAddress, port);

    [ContextMenu("StopClient")]
    public void StopClient() => Client.TryStopClient();
}
