using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ClientUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipAddress;
    [SerializeField] private Button connectButton;
    [SerializeField] private TextMeshProUGUI connectButtonText;
    [SerializeField] private TextMeshProUGUI connectStatus;

    private Client _client;
    private SynchronizationContext _syncContext;
    public bool IsConnected => _client?.IsConnected ?? false;

    public void Setup(Client client)
    {
        _client = client;
        _syncContext = SynchronizationContext.Current;
        _client.IsConnectedChanged -= OnClientConnectedChanged;
        _client.IsConnectedChanged += OnClientConnectedChanged;
        UpdateView();
    }

    public void TearDown()
    {
        _client.IsConnectedChanged -= OnClientConnectedChanged;
        _client = null;
    }

    private void OnClientConnectedChanged(Client client)
    {
        _syncContext.Post(_ => UpdateView(), null);
    }

    public void OnConnectButtonClicked()
    {
        if (IsConnected)
        {
            _client.TryStopClient();
        }
        else
        {
            _client.TryStartClient(ipAddress.text, ConnectionMaker.PORT);
        }
    }

    private void UpdateView()
    {
        connectButtonText.text = !IsConnected ? "Connect" : "Disconnect";
        connectStatus.text = IsConnected ? "Connected" : "Disconnected";
        // connectButton.interactable = !IsConnected;
    }
}
