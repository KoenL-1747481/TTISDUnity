using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
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
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceivedLaptop))
        {
            _packet.Write(SessionManager.clientServer.myId);
            _packet.Write("Sample Username"); // Useless for laptop

            SendTCPDataToServer(_packet);
        }
    }

    public static void SendKinectData(List<Quaternion> boneRotations)
    {
        //Debug.Log("Sending kinect data...");
        using (Packet _packet = new Packet((int)ClientPackets.kinectData))
        {
            _packet.Write(boneRotations);
            _packet.WriteLength();
            foreach (UdpClient cardboard in SessionManager.cardboards.Values)
            {
                try
                {
                    _packet.InsertInt(SessionManager.clientServer.myId); // Insert the client's ID at the start of the packet
                    cardboard.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
                catch (Exception _ex)
                {
                    Debug.Log($"Error sending data to cardboard via UDP: {_ex}");
                }
            }
        }
    }

    #region loop_packets
    public static void SendLoopRecordRequest()
    {
        using (Packet _packet = new Packet((int)ClientPackets.loopRecordRequest))
        {
            SendTCPDataToServer(_packet);
        }
    }
    #endregion



    #endregion
}
