using UnityEngine;
using Network;
using Network.Enums;
using System;
using System.IO;

public class NetworkClient : MonoBehaviour
{
    private readonly string SERVER_IP = "192.168.0.212";
    private readonly int SERVER_PORT = 25566;

    private TcpConnection clientServer; // Client-server connection

    public async void Start()
    {
        print("Trying to connect to server...");
        Tuple<TcpConnection,ConnectionResult> res = await ConnectionFactory.CreateTcpConnectionAsync(SERVER_IP, SERVER_PORT);
        clientServer = res.Item1;
        clientServer.LogIntoStream(File.OpenWrite(Application.persistentDataPath + "CLIENT_LOG.txt"));
        clientServer.EnableLogging = true;
        if (res.Item2 == ConnectionResult.Connected)    
        {
            print("Connected to server!");
            clientServer.ConnectionClosed += ClientServer_ConnectionClosed;
        }
    }
    
    private void ClientServer_ConnectionClosed(CloseReason reason, Connection conn)
    {
        print("Connection with server closed. Reason: " + reason.ToString());
    }

    public void OnApplicationQuit()
    {
        clientServer?.Close(CloseReason.ClientClosed);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
