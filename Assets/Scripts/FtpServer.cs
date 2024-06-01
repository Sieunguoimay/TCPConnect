using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class FtpServer : MonoBehaviour
{
    private TcpListener _listener;

    [ContextMenu("StartServer")]
    public void StartServer()
    {
        _listener = new TcpListener(IPAddress.Any, 21);
        _listener.Start();
        _listener.BeginAcceptTcpClient(OnReceiveResult, _listener);
    }

    [ContextMenu("StopServer")]
    public void StopServer()
    {
        _listener?.Stop();
    }

    private void OnReceiveResult(IAsyncResult result)
    {
        TcpClient client = _listener.EndAcceptTcpClient(result);
        _listener.BeginAcceptTcpClient(OnReceiveResult, _listener);

        var stream = client.GetStream();

        using var writer = new StreamWriter(stream, Encoding.ASCII);
        using var reader = new StreamReader(stream, Encoding.ASCII);

        writer.WriteLine("YOU CONNECTED TO ME");
        writer.Flush();
        writer.WriteLine("I will repeat after you. Send a blank line to quit.");
        writer.Flush();

        string line = null;

        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
        {
            writer.WriteLine("Echoing back: {0}", line);
            writer.Flush();
        }
    }
}
