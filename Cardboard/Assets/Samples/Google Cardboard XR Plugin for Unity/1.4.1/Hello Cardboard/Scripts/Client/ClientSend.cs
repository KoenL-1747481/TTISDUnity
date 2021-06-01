using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPDataToServer(Packet _packet)
    {
        _packet.WriteLength();
        SessionManager.clientServer.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPDataToServer(Packet _packet)
    {
        _packet.WriteLength();
        SessionManager.clientServer.udp.SendData(_packet);
    }

    #region Packets
    /// <summary>Lets the server know that the welcome message was received.</summary>
    public static void WelcomeReceived()
    {
        Debug.Log("Sending welcome received...");
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceivedCardboard))
        {
            _packet.Write(SessionManager.clientServer.myId);
            _packet.Write("Sample Username"); // Not implemented kek
            _packet.Write("Electric Guitar");//_packet.Write(SessionManager.MyInstrument);

            SendTCPDataToServer(_packet);
        }
    }
    #endregion
}
