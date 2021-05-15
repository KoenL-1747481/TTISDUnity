using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;

public class NetworkClient : MonoBehaviour
{
    [SerializeField] private InstrumentSelect instrumentSelector;

    private static readonly string LOCAL_IP_KOEN = "192.168.0.68";
    private static readonly string LOCAL_IP_JEFFREY = "192.168.0.212";
    private readonly string SERVER_IP_KOEN = "94.110.227.197";
    private readonly string SERVER_IP_JEFFREY = "84.193.179.2";

    private readonly int SERVER_PORT = 25566;

    //private TcpConnection clientServer; // Client-server connection

    private bool listening;
    private object peer_lock = new object();

    TcpClient client;
    Connection newTCPConn;

    public void Start()
	{
        ConnectionInfo connInfo = new ConnectionInfo(SERVER_IP_KOEN, SERVER_PORT);
        newTCPConn = TCPConnection.GetConnection(connInfo);
        newTCPConn.SendObject("unk", "diggema");
    }

   /* public IEnumerator klets()
    {
        yield return new WaitForSeconds(1.0f);
        NetworkStream stream = client.GetStream();
        byte uw_dikke_ma = 69;
        print("Sending data");
        stream.WriteByte(uw_dikke_ma);
        StartCoroutine(klets());
    }*/

    /*private void ClientServer_ConnectionClosed(CloseReason reason, Connection conn)
    {
        print("Connection with server closed. Reason: " + reason.ToString());
    }*/

    /*public IEnumerator diggema()
    {
        print("Sending instrument");
        yield return new WaitForSeconds(1.0f);
        sendInstrumentToServer("Keyboard");
        StartCoroutine(diggema());
    }*/

    /*public void OnApplicationQuit()
    {
        clientServer?.Close(CloseReason.ClientClosed);
    }*/

    /*private void PeerInfoReceived(CardboardClientInfo data, Connection connection)
    {
        print("Received peer info from server!");
        Console.WriteLine("Received peer info from server!");
        Console.WriteLine(data.ip + " " + data.instrument);

        lock (peer_lock)
        {
            peerInfoList.Add(data);
            instrumentSelector.addNewPlayer(peerInfoList.Count - 1, data.instrument);
        }
    }*/


    // Update is called once per frame
    void Update()
    {

    }
}
