using UnityEngine;

public class UIDataInjector : MonoBehaviour
{
    [SerializeField] private ClientUI clientUI;
    [SerializeField] private ServerUI serverUI;
    [SerializeField] private ConnectionManager connectionManager;

    private void OnEnable()
    {
        clientUI.Setup(connectionManager.ConnectionMaker.Client);
        serverUI.Setup(connectionManager.ConnectionMaker.Server);
    }

    private void OnDisable()
    {
        clientUI.TearDown();
        serverUI.TearDown();
    }
}