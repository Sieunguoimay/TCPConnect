using System.Net;
using TMPro;
using UnityEngine;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    
    private Connection _connection;

    public void Setup(Connection connection)
    {
        _connection = connection;
        label.text = (connection.Client.Client.RemoteEndPoint as IPEndPoint).Address.ToString();
    }

    public void TearDown()
    {

    }

    public void OnCloseConnectionClicked()
    {
        // _connection.DestroyConnection();
    }
}
