using System;
using System.Net;
using UnityEngine;

public class IPAddressManager
{
    public static readonly int MAIN_PORT = 7004;
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip == null) continue;
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1";
    }

    public static IPEndPoint GetLocalIPEndPoint()
    {
        return new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), MAIN_PORT);
    }
}
