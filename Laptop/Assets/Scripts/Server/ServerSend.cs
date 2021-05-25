using System.Collections;
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
            if (Server.clients[i].player != null)
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
            if (i != _exceptClient && Server.clients[i].player != null)
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

    public static void LoopRecordResponse(int _toClient, bool OK, string msg, int BPM, int bars)
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

    public static void SendLoopResponse(int _toClient, bool OK, string msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.sendLoopResponse))
        {
            _packet.Write(OK);
            _packet.Write(msg);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void AddLoop(int _exceptClient, float[] audio)
    {
        using (Packet _packet = new Packet((int)ServerPackets.addLoop))
        {
            _packet.Write(audio);
            _packet.WriteLength();
            foreach (ServerClient c in Server.clients.Values)
            {
                if (c.player != null && c.player.instrumentType == null && c.id != _exceptClient)
                {
                    Server.clients[c.id].tcp.SendData(_packet);
                }
            }
        }
    }

    public static void StartedRecording(int _exceptClient, int BPM, int Bars)
    {
        int cardboard_id = 0;
        string IP = Server.clients[_exceptClient].player.IP;

        // Find the id of the cardboard matching this laptop IP
        foreach (ServerClient c in Server.clients.Values)
        {
            if (c.player != null && c.player.instrumentType != null)
            {
                if (c.player.IP == IP)
                {
                    cardboard_id = c.id;
                    break;
                }
            }
        }
        if (cardboard_id == 0) // If we didn't find matching cardboard, don't send started recording request
            return;
        using (Packet _packet = new Packet((int)ServerPackets.startedRecording))
        {
            _packet.Write(cardboard_id);
            _packet.Write(BPM);
            _packet.Write(Bars);

            SendTCPDataToAll(_exceptClient, _packet);
        }
    }

    public static void UndoLoop(int _exceptClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.undoLoop))
        {
            SendTCPDataToAll(_exceptClient, _packet);
        }
    }
    #endregion
}
