using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network.Logging;
using System.IO;

public class NetworkClient : MonoBehaviour
{
    [SerializeField] private InstrumentSelect instrumentSelector;

    private readonly string SERVER_IP = "192.168.0.212";
    private readonly int SERVER_PORT = 25566;

    private TcpConnection clientServer; // Client-server connection

    private bool listening;
    private object peer_lock = new object();

    List<CardboardClientInfo> peerInfoList = new List<CardboardClientInfo>();
    ClientConnectionContainer container;

    TcpClient client;

    public async void Start()
    {
        /*client = new TcpClient();
        await client.ConnectAsync(IPAddress.Parse(SERVER_IP), SERVER_PORT);
        StartCoroutine(klets());*/
        Tuple<TcpConnection,ConnectionResult> res = await ConnectionFactory.CreateTcpConnectionAsync(SERVER_IP, SERVER_PORT);
        clientServer = res.Item1;
        print("Adding file diggema");
        clientServer.LogIntoStream(File.OpenWrite(Application.persistentDataPath + "DIGGE_CLIENT_LOG.txt"));
        clientServer.EnableLogging = true;
        if (res.Item2 == ConnectionResult.Connected)    
        {
            print("Connected to server!");
            clientServer.ConnectionClosed += ClientServer_ConnectionClosed;
            //clientServer?.RegisterPacketHandler<CardboardClientInfo>(PeerInfoReceived, this);
            //clientServer.KeepAlive = true;
            //sendInstrumentToServer("Keyboard");
            //StartCoroutine(diggema());
        }
    }

    private void Container_ConnectionLost(Connection conn, ConnectionType type, CloseReason reason)
    {
        print("Connection with server closed. Type: " + type.ToString() + ", Reason: " + reason.ToString());
    }

    private void Container_ConnectionEstablished(Connection conn, ConnectionType type)
    {
        print("Connection established with server! Type: " + type.ToString());
        conn.LogIntoStream(File.OpenWrite(Application.dataPath + "/DIGGE_CLIENT_LOG.txt"));
        conn.EnableLogging = true;
    }

    public IEnumerator klets()
    {
        yield return new WaitForSeconds(1.0f);
        NetworkStream stream = client.GetStream();
        byte uw_dikke_ma = 69;
        print("Sending data");
        stream.WriteByte(uw_dikke_ma);
        StartCoroutine(klets());
    }

    private void ClientServer_ConnectionClosed(CloseReason reason, Connection conn)
    {
        print("Connection with server closed. Reason: " + reason.ToString());
    }

    public IEnumerator diggema()
    {
        print("Sending instrument");
        yield return new WaitForSeconds(1.0f);
        sendInstrumentToServer("Keyboard");
        StartCoroutine(diggema());
    }

    public void OnApplicationQuit()
    {
        clientServer?.Close(CloseReason.ClientClosed);
    }

    public void sendInstrumentToServer(string instrumentName)
    {
        print("send " + instrumentName);
        //ThreadPool.QueueUserWorkItem((object a) =>
        //{
            clientServer.Send(new InstrumentName(instrumentName));
        //});
    }

    private void PeerInfoReceived(CardboardClientInfo data, Connection connection)
    {
        print("Received peer info from server!");
        Console.WriteLine("Received peer info from server!");
        Console.WriteLine(data.ip + " " + data.instrument);

        lock (peer_lock)
        {
            peerInfoList.Add(data);
            instrumentSelector.addNewPlayer(peerInfoList.Count - 1, data.instrument);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
