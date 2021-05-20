using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using UnityEngine;

public class Server
{
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static UdpClient udpListener;


    /// <summary>Starts the server.</summary>
    public static void Start()
    {
        Debug.Log("Starting server...");
        InitializeServerData();

        udpListener = new UdpClient(Constants.CARDBOARD_PORT);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on port {Constants.CARDBOARD_PORT}.");
    }

    
    public static void Dispose()
    {
        udpListener?.Close();
        udpListener?.Dispose();
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

                if (_clientId <= 0)
                {
                    return;
                }
                /* Check if we have spawned the cardboard in the scene with this id */
                List<int> cardboard_ids = new List<int>();
                Debug.Log("Cardboard ids: " + cardboard_ids.ToString());
                Debug.Log("Received cardboard id: " + _clientId);
                foreach (Player c in SessionManager.cardboards)
                {
                    cardboard_ids.Add(c.id);
                }
                if (cardboard_ids.Contains(_clientId))
                {
                    HandleData(_clientId, _packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error receiving UDP data: {_ex}");
        }
    }

    /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
    /// <param name="_packetData">The packet containing the recieved data.</param>
    private static void HandleData(int _clientId, Packet _packetData)
    {
        int _packetLength = _packetData.ReadInt();
        byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

        ThreadManager.ExecuteOnMainThread(() =>
        {
            using (Packet _packet = new Packet(_packetBytes))
            {
                int _packetId = _packet.ReadInt();
                Server.packetHandlers[_packetId](_clientId, _packet); // Call appropriate method to handle the packet
            }
        });
    }

    /// <summary>Initializes all necessary server data.</summary>
    private static void InitializeServerData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.kinectData, ServerHandle.KinectDataReceived},
            };
        Debug.Log("Initialized packets.");
    }
}
