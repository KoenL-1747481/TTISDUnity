using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        SessionManager.clientServer.myId = _myId;

        // Spawn myself and send welcome received
        SessionManager.instance.AddCardboard(new Player(_myId, "Sample Username", ((IPEndPoint)SessionManager.clientServer.tcp.socket.Client.LocalEndPoint).Address.ToString(), SessionManager.MyInstrument));
        ClientSend.WelcomeReceived();

        // Now that we have the client's id, connect UDP
        SessionManager.clientServer.udp.Connect(((IPEndPoint)SessionManager.clientServer.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void AddCardboard(Packet _packet)
    {
        Debug.Log("Adding cardboard...");
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        string _IP = _packet.ReadString();
        string _instrumentType = _packet.ReadString();


        SessionManager.instance.AddCardboard(new Player(_id, _username, _IP, _instrumentType));
    }
}
