﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using System.Text;

using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System.Net.Sockets;
using System.Net;
using System.Threading;

class Server_Old: MonoBehaviour
{
    public static readonly int MAX_PLAYERS = 4;
   // private static ServerConnectionContainer server;
    private static readonly string LOCAL_IP_KOEN = "192.168.0.68";
    private static readonly string LOCAL_IP_JEFFREY = "192.168.0.212";
    private static readonly string EXTERNAL_IP_KOEN = "94.110.227.197";
    private readonly string EXTERNAL_IP_JEFFREY = "84.193.179.2";
    private static readonly int PORT = 25566;

    /* Session settings */
    private static int BPM = 120;
    private static int Bars = 4;

    private static List<Tuple<string, string>> cardboardConnectionInfo = new List<Tuple<string, string>>();
    //private static List<Connection> cardboardConnections = new List<Connection>();
    private static object cardboardClient_lock = new object();

    TcpListener listener;

    public void StartServer()
    {
        /*Dispose();
        server = ConnectionFactory.CreateServerConnectionContainer(PORT);
        server.ConnectionEstablished += ConnectionEstablished;
        server.ConnectionLost += ConnectionLost;

        server.AllowUDPConnections = false;
        server.Start();
        print("listening");*/
        listener = new TcpListener(IPAddress.Parse(LOCAL_IP_JEFFREY), PORT);
        listener.Start();
        print("Listening...");
        ThreadPool.QueueUserWorkItem((object a) =>
        {
            TcpClient client = listener.AcceptTcpClient();
            print("Connected!");
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[2048];
            int amount_read;
            while (true)
            {
                amount_read = stream.Read(buffer, 0, buffer.Length);
                for (int i = 0; i < amount_read; i++)
                {
                    print(buffer[i].ToString());
                }
            }
         });

        //NetworkComms.AppendGlobalIncomingPacketHandler<string>("unk", onReceive);
        //Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Parse(LOCAL_IP_JEFFREY), PORT));

    }

    private void onReceive(PacketHeader packetHeader, Connection connection, string incomingObject)
    {
        print(incomingObject);
    }

    

    public void Start()
    {
        StartServer();
    }

    private void OnDestroy()
    {
        NetworkComms.Shutdown();
    }

    /*public void Dispose()
    {
        server?.Stop();
        server?.CloseConnections(CloseReason.ServerClosed);
    }*/

   /* private void ConnectionLost(Connection connection, ConnectionType type, CloseReason reason)
    {
        print("connection lost");
        print(type.ToString() + "Connection lost, close reason: " + reason.ToString());
        Console.WriteLine($"{server.Count} {type.ToString()} Connection lost {connection.IPRemoteEndPoint.Port}. Reason {reason.ToString()}");
        if (server.Count == MAX_PLAYERS - 1)
            server.StartTCPListener();
        //UpdateP2P();
    }
   */
   /* private void ConnectionEstablished(Connection connection, ConnectionType type)
    {
        print("New connection!");
        print($"{server.Count} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");
        if (server.Count == MAX_PLAYERS)
            server.Stop();
        //connection.KeepAlive = true;
        connection.LogIntoStream(File.OpenWrite("DIGGE_SERVER_LOG.txt"));
        connection.EnableLogging = true;
        connection.RegisterPacketHandler<InstrumentName>(InstrumentNameReceived, this);

        UpdateP2P();
    }*/


    /*private void InstrumentNameReceived(InstrumentName data, Connection connection)
    {
        print("received instrument");
        Console.WriteLine("Received instrument info from client!@@@@@@@@@@@@@@");
        print(data.name);
        lock (cardboardClient_lock)
        {
            cardboardConnections.Add(connection);
            cardboardConnectionInfo.Add(new Tuple<string, string>(connection.IPRemoteEndPoint.ToString(),data.name.ToString()));
            updateCardBoardClients();
        }
    }*/

   /* private void updateCardBoardClients()
    {
        //send the new client's data to all older clients
        for (int i=0;i< cardboardConnectionInfo.Count-1;i++)
        {
            cardboardConnections[i].Send(new CardboardClientInfo(cardboardConnectionInfo[cardboardConnectionInfo.Count-1].Item1, cardboardConnectionInfo[cardboardConnectionInfo.Count - 1].Item2));
        }
        //send the data from all clients to the new client
        for (int i = 0; i < cardboardConnectionInfo.Count; i++)
        {
            cardboardConnections[cardboardConnectionInfo.Count - 1].Send(new CardboardClientInfo(cardboardConnectionInfo[i].Item1, cardboardConnectionInfo[i].Item2));
        }
    }*/

   /* private void UpdateP2P()
    {
        // Get all connected clients
        var connections = server.TCP_Connections;
        List<string> ip_addresses = new List<string>();
        // Get all ip addresses of clients
        foreach (var conn in connections)
            ip_addresses.Add(conn.IPRemoteEndPoint.Address.ToString());
        // If server also is part of p2p network, replace local ip with external ip
        int i = ip_addresses.IndexOf("127.0.0.1");
        if (i != -1)
            ip_addresses[i] = SERVER_IP;
        else
        {
            i = ip_addresses.IndexOf("192.168.0.1");
            if (i != -1)
                ip_addresses[i] = SERVER_IP;
        }
        // Send all ip addresses except their own to every client
        foreach (var conn in connections)
        {
            // Handle previous ip edit
            List<string> ip_addresses_to_send = ip_addresses.Where((v, ind) => v != conn.IPRemoteEndPoint.Address.ToString()).ToList();
            if (ip_addresses_to_send.Count == ip_addresses.Count)
            {
                // Nothing changed so we are sending to local host
                ip_addresses_to_send.Remove(SERVER_IP);
            }
            //conn.Send(new P2PInfo(ip_addresses_to_send));
        }
    }*/
} 

