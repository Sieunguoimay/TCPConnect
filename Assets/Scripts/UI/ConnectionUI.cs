using System.Collections.Generic;
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
        label.text = connection.RemoteEndPoint.Address.ToString();
    }

    public void TearDown()
    {

    }

    public void OnCloseConnectionClicked()
    {
        _connection.Disconnect();
    }

    public void OnSendFileClicked()
    {
        if (ApplicationStartUp.Instance.ToolBoxManager.TryGetToolBox(_connection, out var toolbox))
        {
            toolbox.FileTransferTool.StartMakingDataConnection("file_path_example", new Dictionary<string, string> { { "file_name", "file_name_example" } });
        }
    }
}
