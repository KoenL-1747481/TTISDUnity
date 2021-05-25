using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceivedLaptop(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoSession(_username);
    }

    public static void WelcomeReceivedCardboard(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();
        string _instrumentType = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoSession(_username, _instrumentType);
    }

    public static void LoopRecordRequest(int _fromClient, Packet _packet)
    {
        Debug.Log("LoopRecordRequest received.");
        Server.OnRecordRequest(_fromClient);
    }
    
    public static void SendLoopRequest(int _fromClient, Packet _packet)
    {
        Debug.Log("LoopRecordRequest received.");
        float[] audio = _packet.ReadFloats();
        Debug.Log("Receiving Loop. Audio length: " + audio.Length);
        Server.OnSendLoopRequest(_fromClient, audio);
    }

    public static void UndoLoopRequest(int _fromClient, Packet _packet)
    {
        ServerSend.UndoLoop(_fromClient);
    }
}
