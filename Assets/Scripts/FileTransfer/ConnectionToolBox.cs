using System;

public class ConnectionToolBox : IDisposable
{
    private readonly Connection c;
    public FileTransferTool FileTransferTool { get; } = new();

    public ConnectionToolBox(Connection c)
    {
        this.c = c;
        FileTransferTool.Setup();
    }

    public void Dispose()
    {
        FileTransferTool.TearDown();
    }
}
