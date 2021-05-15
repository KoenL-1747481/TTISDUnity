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

    private static readonly string LOCAL_IP2 = "192.168.0.68";
    private readonly string SERVER_IP = "94.110.227.197";
    private readonly int SERVER_PORT = 25566;

    //private TcpConnection clientServer; // Client-server connection

    private bool listening;
    private object peer_lock = new object();

   // List<CardboardClientInfo> peerInfoList = new List<CardboardClientInfo>();

    TcpClient client;
    Connection newTCPConn;

    public void Start()
    {
        /*client = new TcpClient();
        print("kaas");
        await client.ConnectAsync(IPAddress.Parse(SERVER_IP), SERVER_PORT);
        print("kaasanus");
        StartCoroutine(klets());*/
        /*ConnectionResult res = new ConnectionResult();

        clientServer = ConnectionFactory.CreateTcpConnection(SERVER_IP, SERVER_PORT, out res);
        if (res == ConnectionResult.Connected)
        {
            clientServer?.RegisterPacketHandler<CardboardClientInfo>(PeerInfoReceived, this);
            clientServer.KeepAlive = true;
            clientServer.ConnectionClosed += ClientServer_ConnectionClosed;
            print("Connected to keyboard");
            sendInstrumentToServer("Keyboard");
            //StartCoroutine(diggema());
        }*/


        //NetworkComms.SendObject("Message", SERVER_IP, SERVER_PORT, "kaasaas");
        ConnectionInfo connInfo = new ConnectionInfo(LOCAL_IP2, SERVER_PORT);
        newTCPConn = TCPConnection.GetConnection(connInfo);
        newTCPConn.SendObject("unk", "diggema");
        /*newTCPConn.AppendIncomingPacketHandler<string>("StringMessage",
            MethodToRunForStringMessage,
            NetworkComms.DefaultSendReceiveOptions);
        }*/


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

    public void sendInstrumentToServer(string instrumentName)
    {
        print("send " + instrumentName);
        newTCPConn.SendObject("unk", instrumentName);
        //clientServer.Send(new InstrumentName(instrumentName));
    }

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
