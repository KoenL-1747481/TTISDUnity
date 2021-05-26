using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using TTISDProject;
using UnityEngine;
using UnityEngine.UI;

public class ClientHandle : MonoBehaviour
{
    private static Timer timer;

    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        SessionManager.clientServer.myId = _myId;
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

    public static void AddLaptop(Packet _packet)
    {
        Debug.Log("Adding laptop...");

        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        string _IP = _packet.ReadString();

        SessionManager.instance.AddLaptop(new Player(_id, _username, _IP));
    }

    public static void LoopRecordResponse(Packet _packet)
    {
        Debug.Log("Received record response.");
        bool OK = _packet.ReadBool();
        string msg = _packet.ReadString();
        int BPM = _packet.ReadInt();
        int bars = _packet.ReadInt();

        if (OK)
        {
            LoopRecorder.StartRecording(BPM, bars);
        } else
        {
            Debug.Log(msg);
        }
    }

    public static void SendLoopResponse(Packet _packet)
    {
        Debug.Log("Received SendLoopResponse");
        bool OK = _packet.ReadBool();
        if (OK)
        {
            AudioHandler.AddLoop(LoopRecorder.recorded_audio);
        } else
        {
            string msg = _packet.ReadString();
            Debug.Log(msg);
        }
    }

    public static void AddLoop(Packet _packet)
    {
        float[] audio = _packet.ReadFloats();
        Debug.Log("Received AddLoop. Audio Length: " + audio.Length);
        AudioHandler.AddLoop(audio);
    }

    public static void StartAddLoop(Packet _packet)
    {
        int loop_length = _packet.ReadInt();
        Debug.Log("Setting up loop with length: " + loop_length);
        AudioHandler.SetLoopLength(loop_length);
    }

    public static void PartAddLoop(Packet _packet)
    {
        Debug.Log("Received UDP data.");
        float[] audio = _packet.ReadFloats();
        SessionManager.instance.PartAddLoop(audio);
    }

    public static void StartedRecording(Packet _packet)
    {
        int cardboardId = _packet.ReadInt();
        int BPM = _packet.ReadInt();
        int Bars = _packet.ReadInt();

        RecordButton.btn.interactable = false;
        double clickInterval = (1.0 / (BPM / 60.0)) * 1000.0;
        double timeoutInterval = clickInterval * 4.0 * (Bars + 1);
        timer = new Timer(timeoutInterval);
        timer.Elapsed += (s_, e_) =>
        {
            ThreadManager.ExecuteOnMainThread(() =>
            {
                RecordButton.btn.interactable = true;
            });
        };
        timer.AutoReset = false;
        timer.Start();
    }

    public static void UndoLoop(Packet _packet)
    {
        Debug.Log("Recieved UNDO");
        AudioHandler.UndoLoop();
    }
}
