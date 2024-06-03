using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ConnectorUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipAddress;
    [SerializeField] private Button connectButton;

    private Connector _connector;

    public void Setup(Connector client)
    {
        _connector = client;
        _connector.ConnectStatusChanged -= OnConnectStatusChanged;
        _connector.ConnectStatusChanged += OnConnectStatusChanged;
    }

    public void TearDown()
    {
        _connector.ConnectStatusChanged -= OnConnectStatusChanged;
        _connector = null;
    }

    private void OnConnectStatusChanged(Connector provider)
    {
        connectButton.interactable = !_connector.IsConnecting;
    }

    public void OnConnectButtonClicked()
    {
        if (string.IsNullOrEmpty(ipAddress.text)) return;
        _connector.TryConnect(ipAddress.text, IPAddressManager.PORT);
    }
}
