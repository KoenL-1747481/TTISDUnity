﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    #region GeneralSend
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }
#endregion

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_msg">The message to send.</param>
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
        Debug.Log("Sent welcome message!");
    }

    public static void AddCardboard(int _toClient, Player player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.addCardboard))
        {
            _packet.Write(player.id);
            _packet.Write(player.username);
            _packet.Write(player.IP);
            _packet.Write(player.instrumentType);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void AddLaptop(int _toClient, Player player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.addLaptop))
        {
            _packet.Write(player.id);
            _packet.Write(player.username);
            _packet.Write(player.IP);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void RecordResponse(int _toClient, bool OK, string msg, int BPM, int bars)
    {
        using (Packet _packet = new Packet((int)ServerPackets.loopRecordResponse))
        {
            _packet.Write(OK);
            _packet.Write(msg);
            _packet.Write(BPM);
            _packet.Write(bars);

            SendTCPData(_toClient, _packet);
        }
    }
    #endregion
}
