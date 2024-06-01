using System;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public ConnectionMaker ConnectionMaker { get; } = new();

    private void Start()
    {
        ConnectionMaker.Begin();
    }

    private void OnDestroy()
    {
        ConnectionMaker.End();
    }
}
