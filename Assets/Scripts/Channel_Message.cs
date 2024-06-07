using System;
using System.Net.Sockets;
using System.Text;

public class Channel_Message : Channel
{
    private readonly StringBuilder stringBuilder = new();
    public event Action<Channel_Message, string> ReceivedMessageEvent;

    public Channel_Message(NetworkStream stream, Action<Channel> disconnectCallback) 
        : base(stream, disconnectCallback)
    {
    }

    protected override void HandleReadData(byte[] data)
    {
        base.HandleReadData(data);
        stringBuilder.Append(data);

        var message = stringBuilder.ToString();
        if (message.Contains("\n"))
        {
            SyncContext.Post(_ => ReceivedMessageEvent?.Invoke(this, message), null);
        }
    }
}
