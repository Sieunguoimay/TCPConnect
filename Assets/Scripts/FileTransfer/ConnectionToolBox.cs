using System;

public class ConnectionToolBox : IDisposable
{
    public FileTransferTool FileTransferTool { get; } = new();

    public ConnectionToolBox()
    {
    }

    public void SetupTools(Connection c)
    {
        FileTransferTool.Setup(new FileTransferTool.Dependencies(c));
    }

    public void TearDownTools()
    {
        FileTransferTool.TearDown();
    }

    public void Dispose()
    {
        FileTransferTool.TearDown();
    }
}
