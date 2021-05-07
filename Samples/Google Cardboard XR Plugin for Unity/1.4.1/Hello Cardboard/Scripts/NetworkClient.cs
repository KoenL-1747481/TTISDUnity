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


public class NetworkClient : MonoBehaviour
{
    [SerializeField] private InstrumentSelect instrumentSelector;

    private readonly string SERVER_IP = "141.135.129.110";
    private readonly int SERVER_PORT = 25566;
    private readonly int P2P_PORT = 25565;

    private ClientConnectionContainer clientServer; // Client-server connection

    private bool listening;
    private object peer_lock = new object();

    List<PeerInfo> peerInfoList = new List<PeerInfo>();

    public NetworkClient()
    {
        clientServer = ConnectionFactory.CreateClientConnectionContainer(SERVER_IP, SERVER_PORT);
        clientServer.ConnectionEstablished += ConnectionEstablished;
        listening = true;
    }

    public void sendInstrumentToServer(string instrumentName)
    {
        clientServer.Send(new InstrumentName(instrumentName));
        /*PeerInfo data = new PeerInfo("kaas", "Keyboard");
        peerInfoList.Add(data);
        instrumentSelector.spawnPlayer(peerInfoList.Count - 1, data.instrument);*/
    }

    private void ConnectionEstablished(Connection connection, ConnectionType type)
    {
        Console.WriteLine($"{type} Connection established with server.");
        connection.KeepAlive = true;

        // Register packet handlers
        connection.RegisterPacketHandler<PeerInfo>(PeerInfoReceived, this);
    }

    private void PeerInfoReceived(PeerInfo data, Connection connection)
    {
        Console.WriteLine("Received peer info from server!");
        Console.WriteLine(data.ip+" "+data.instrument);

        lock (peer_lock)
        {
            peerInfoList.Add(data);
            instrumentSelector.spawnPlayer(peerInfoList.Count - 1, data.instrument);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
