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

    private static bool ReceivingLoop = false;
    private static float[] receive_buffer = new float[10000000];
    private static int buffer_pos = 0;

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
        ReceivingLoop = true;
    }

    public static void PartAddLoop(Packet _packet)
    {
        float[] audio = _packet.ReadFloats();
        Array.Copy(audio, 0, receive_buffer, buffer_pos, audio.Length);
        buffer_pos += audio.Length;
    }

    public static void EndAddLoop(Packet _packet)
    {
        ReceivingLoop = false;
        int loop_length = buffer_pos;
        buffer_pos = 0;

        float[] audio = new float[loop_length];
        Array.Copy(receive_buffer, 0, audio, 0, loop_length);
        AudioHandler.AddLoop(audio);
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
