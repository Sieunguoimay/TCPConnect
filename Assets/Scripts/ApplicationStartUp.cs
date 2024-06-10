using UnityEngine;

public class ApplicationStartUp : MonoBehaviour
{
    public ConnectionManager ControlConnectionManager { get; } = new();
    public ConnectionToolBoxManager ToolBoxManager { get; } = new();
    public static ApplicationStartUp Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartUp();
    }

    private void OnDestroy()
    {
        ShutDown();
    }

    private void OnApplicationQuit()
    {
        ShutDown();
    }

    private void StartUp()
    {
        ControlConnectionManager.Setup();
        ControlConnectionManager.Listener.TryStartServer(IPAddressManager.GetLocalIPEndPoint());

        ToolBoxManager.Setup(ControlConnectionManager.Container);
    }

    private void ShutDown()
    {
        ToolBoxManager.TearDown();

        ControlConnectionManager.Listener.TryStopServer();
        ControlConnectionManager.TearDown();
    }
}
