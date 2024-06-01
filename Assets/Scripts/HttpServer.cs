using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class HttpServer : MonoBehaviour
{
    private const int PORT = 144;
    private HttpListener _listener;

    [ContextMenu("StartServer")]
    public void StartServer()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://*:{PORT}/");
        _listener.Start();
        Receive();
    }

    [ContextMenu("StopServer")]
    public void StopServer()
    {
        _listener.Stop();
    }

    private void Receive()
    {
        _listener.BeginGetContext(new AsyncCallback(OnReceiveResult), _listener);
    }

    private void OnReceiveResult(IAsyncResult result)
    {
        if (_listener.IsListening)
        {
            var context = _listener.EndGetContext(result);
            var request = context.Request;

            PrintRequest(request);
            ProvideOutput(context);

            Receive();
        }
    }

    private void ProvideOutput(HttpListenerContext context)
    {
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = "text/plain";
        response.OutputStream.Write(new byte[] { }, 0, 0);
        response.OutputStream.Close();
    }

    private void PrintRequest(HttpListenerRequest request)
    {
        Debug.Log($"{request.HttpMethod} {request.Url}");

        if (request.HasEntityBody)
        {
            var body = request.InputStream;
            var encoding = request.ContentEncoding;
            var reader = new StreamReader(body, encoding);
            if (request.ContentType != null)
            {
                Debug.Log("Client data content type {request.ContentType}");
            }
            Debug.Log("Client data content length {request.ContentLength64}");

            Debug.Log("Start of data:");
            string s = reader.ReadToEnd();
            Debug.Log(s);
            Debug.Log("End of data:");
            reader.Close();
            body.Close();
        }
    }

}
