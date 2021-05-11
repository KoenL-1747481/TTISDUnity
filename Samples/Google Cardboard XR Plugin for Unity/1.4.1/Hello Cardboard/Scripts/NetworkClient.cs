using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
//using Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public class NetworkClient : MonoBehaviour
{
    /*[SerializeField] private InstrumentSelect instrumentSelector;

    private readonly string SERVER_IP = "127.0.0.1";
    private readonly int SERVER_PORT = 25566;

    private TcpConnection clientServer; // Client-server connection

    private bool listening;
    private object peer_lock = new object();

    List<CardboardClientInfo> peerInfoList = new List<CardboardClientInfo>();*/

   /* public void Start()
    {
        ConnectionResult res = new ConnectionResult();
        clientServer =  ConnectionFactory.CreateTcpConnection(SERVER_IP, SERVER_PORT, out res);
        clientServer?.RegisterPacketHandler<CardboardClientInfo>(PeerInfoReceived, this);
        clientServer.KeepAlive = true;
    }

    public void OnApplicationQuit()
    {
        clientServer?.Close(CloseReason.ClientClosed);

        /*clientServer?.Shutdown(CloseReason.ClientClosed);
        clientServer?.Dispose();*/
    /*}

    public void sendInstrumentToServer(string instrumentName)
    {
        clientServer.Send(new InstrumentName(instrumentName));
      
    }

    /*private void ConnectionEstablished(Connection connection, ConnectionType type)
    {
        print("Connection established with server.");
        connection.KeepAlive = true;

        // Register packet handlers
        connection.RegisterPacketHandler<CardboardClientInfo>(PeerInfoReceived, this);
    }

    private void PeerInfoReceived(CardboardClientInfo data, Connection connection)
    {
        Console.WriteLine("Received peer info from server!");
        Console.WriteLine(data.ip+" "+data.instrument);

        lock (peer_lock)
        {
            peerInfoList.Add(data);
            instrumentSelector.spawnPlayer(peerInfoList.Count - 1, data.instrument);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
