using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using UnityEngine;

public class Server
{
    private static int DEFAULT_BPM = 120;
    private static int DEFAULT_BARS = 4;

    public static Dictionary<int, ServerClient> clients = new Dictionary<int, ServerClient>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    /* Session settings */
    private static int BPM = DEFAULT_BPM;
    private static int Bars = DEFAULT_BARS;
    private static ServerClient RecordingPlayer = null;
    private static Timer RecordTimeoutTimer = null;
    private static bool UndoAllowed = true;

    /// <summary>Starts the server.</summary>
    public static void Start()
    {
        Debug.Log("Starting server...");
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Constants.SERVER_PORT);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

        udpListener = new UdpClient(Constants.SERVER_PORT);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on port {Constants.SERVER_PORT}.");
    }

    public static void OnUndoLoopRequest()
    {
        if (UndoAllowed)
        {
            ServerSend.UndoLoop();
            UndoAllowed = false;
            var resetTimer = new System.Timers.Timer(5000);
            resetTimer.Elapsed += (a, b) =>
            {
                UndoAllowed = true;
            };
            resetTimer.AutoReset = false;
            resetTimer.Start();
        }
    }

    public static void OnRecordRequest(int clientId)
    {
        Debug.Log("RecordRequest received.");
        if (RecordingPlayer != null)
        {
            Debug.Log("Someone is already recording!");
            // TODO: Send bool false 
            ServerSend.LoopRecordResponse(clientId, false, "Someone is already recording!", BPM, Bars);
        }
        else
        {
            Debug.Log("Recording is allowed.");
            RecordingPlayer = clients[clientId];
            // Send loop record response to the requester
            ServerSend.LoopRecordResponse(clientId, true, "OK", BPM, Bars);
            // Send loop record started to everyone but the requester
            ServerSend.StartedRecording(clientId, BPM, Bars);
            // If no SendLoopRequest after certain time, then timeout and reset current record request
            /*double clickInterval = (1.0 / (BPM / 60.0)) * 1000.0;
            double timeoutInterval = clickInterval * 4.0 * (Bars + 3.0);
            RecordTimeoutTimer = new Timer(timeoutInterval);
            RecordTimeoutTimer.Elapsed += (s, e_) =>
            {
                RecordingPlayer = null;
                RecordTimeoutTimer.Stop();
                RecordTimeoutTimer.Close();
                Debug.Log("Record request timed out!");
            };
            RecordTimeoutTimer.Start();*/
        }
    }

    public static void OnSendLoopRequest(int clientId, float[] audio) 
    {
        if (clientId == RecordingPlayer.id)
        {
            Debug.Log("Players match.");
            RecordingPlayer = null;
            /*RecordTimeoutTimer?.Stop();
            RecordTimeoutTimer?.Close();*/

            // Send response to the requester
            ServerSend.SendLoopResponse(clientId, true, "OK");
            // Send loop to everyone except the requester
            ServerSend.AddLoopUDP(clientId, audio);
            // Save the loop server side as well, for if someone joins after loops are recorded.
            // TODO: doesn't matter atm
        }
        else
        {
            Debug.Log("Players don't match.");
            ServerSend.SendLoopResponse(clientId, false, "You didn't initiate a record request, or the request timed out.");
        }
    }

    public static void Dispose()
    {
        tcpListener?.Stop();
        udpListener?.Close();
        udpListener?.Dispose();
        foreach (ServerClient c in clients.Values)
        {
            c?.Disconnect();
        }
        RecordTimeoutTimer?.Stop();
        RecordTimeoutTimer?.Close();
        RecordingPlayer = null;
        BPM = DEFAULT_BPM;
        Bars = DEFAULT_BARS;
    }

    /// <summary>Handles new TCP connections.</summary>
    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }

        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    /// <summary>Receives incoming UDP data.</summary>
    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }

                if (clients[_clientId].udp.endPoint == null)
                {
                    // If this is a new connection
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    // Ensures that the client is not being impersonated by another by sending a false clientID
                    clients[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error receiving UDP data: {_ex}");
        }
    }

    /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
    /// <param name="_clientEndPoint">The endpoint to send the packet to.</param>
    /// <param name="_packet">The packet to send.</param>
    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }

    /// <summary>Initializes all necessary server data.</summary>
    private static void InitializeServerData()
    {
        for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
        {
            clients.Add(i, new ServerClient(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceivedCardboard, ServerHandle.WelcomeReceivedCardboard },
                { (int)ClientPackets.welcomeReceivedLaptop, ServerHandle.WelcomeReceivedLaptop},
                { (int)ClientPackets.loopRecordRequest, ServerHandle.LoopRecordRequest},
                { (int)ClientPackets.sendLoopRequest, ServerHandle.SendLoopRequest},
                { (int)ClientPackets.undoLoopRequest, ServerHandle.UndoLoopRequest},
            };
        Debug.Log("Initialized packets.");
    }
}
