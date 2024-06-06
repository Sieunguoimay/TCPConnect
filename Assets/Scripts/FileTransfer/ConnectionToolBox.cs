using System;

public class ConnectionToolBox : IDisposable
{
    private readonly IConnectionTool[] _tools = new IConnectionTool[] { new FileTransfer() };

    public ConnectionToolBox(Connection connection)
    {
        foreach (var t in _tools)
        {
            t.Setup(connection);
        }
    }

    public void Dispose()
    {
        foreach (var t in _tools)
        {
            t.TearDown();
        }
    }
}