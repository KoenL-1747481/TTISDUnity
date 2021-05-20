using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TTISDProject;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager instance;

    public static Client clientServer;
    public static Dictionary<int, Player> players = new Dictionary<int, Player>();
    private static Dictionary<Player, UdpClient> laptopPeers = new Dictionary<Player, UdpClient>(); // Peers to send audio data to
    public static Dictionary<Player, Client> cardboards = new Dictionary<Player, Client>(); // Cardboards to send kinect data to

    private static UdpClient p2p_listener; // Listener that receives peer audio data
    private static bool listening;
    private static byte[] byte_buffer = new byte[1024 * 16];
    private static float[] float_buffer = new float[1024 * 16];

    private void OnApplicationQuit()
    {
        clientServer?.Disconnect();
        players?.Clear();
        foreach (UdpClient socket in laptopPeers.Values)
        {
            socket?.Close();
            socket?.Dispose();
        }
        laptopPeers?.Clear();
        p2p_listener?.Close();
        p2p_listener?.Dispose();
    }

    public void Start()
    {
        clientServer = new Client(Constants.SERVER_IP, Constants.SERVER_PORT);
        clientServer.ConnectToServer();
        p2p_listener = new UdpClient(Constants.P2P_PORT);

        listening = true;
        ThreadPool.QueueUserWorkItem(AudioListenerThread);

    }

    private static void AudioListenerThread(object state)
    {
        Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
        try
        {
            while (listening)
            {
                byte[] audio = p2p_listener.Receive(ref endpoint);
                Debug.Log("Received audio");
                // Get the player from the IP
                int peer_id = -1;
                foreach (Player p in laptopPeers.Keys)
                {
                    if (p.IP == endpoint.Address.ToString())
                    {
                        peer_id = p.id;
                        break;
                    }
                }
                if (peer_id != -1)
                { // We found the player
                  // Convert to float array
                    Buffer.BlockCopy(audio, 0, float_buffer, 0, audio.Length);
                    // Play it
                    AudioHandler.PlayPlayerAudio(float_buffer, audio.Length / sizeof(float), peer_id);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public static void SendAudioToPeers(float[] audio, int count)
    {
        // Convert float data to bytes
        int amount_bytes = count * sizeof(float);
        Buffer.BlockCopy(audio, 0, byte_buffer, 0, amount_bytes);
        // Send to all peers
        foreach (var peer in laptopPeers.Values)
        {
            try
            {
                Debug.Log("Sending audio to: " + peer.Client.RemoteEndPoint.ToString());
                peer.Send(byte_buffer, amount_bytes);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }

    #region loop_stuff
    public void TryRecordLoop()
    {
        if (!LoopRecorder.IsRecording() && clientServer != null)
        {
            ClientSend.SendLoopRecordRequest();
            /*try
            {
                response = await clientServer.SendAsync<RecordResponse>(new RecordRequest());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            if (response.OK)
            {
                LoopRecorder.StartRecording(response.BPM, response.Bars);
            }
            else
            {
                Console.WriteLine(response.Message);
            }*/
        }
    }

    public static void TrySendLoop(float[] audio)
    {
        Console.WriteLine("Audio recorded. Size: " + audio.Length);
        if (clientServer != null)
        {
            // TODO: Send the loop to server
            //response = await clientServer.SendAsync<SendLoopResponse>(new SendLoopRequest(audio));

            // TODO: handle the response
            //AudioHandler.AddLoop(audio);
        }
    }

    // This request contains a new loop that needs to be played
    private static void LoopUpdateRequestReceived(/*LoopUpdate req, Connection connection*/)
    {
        // TODO: when new loop has been recorded, handle packet
        //AudioHandler.AddLoop(req.Audio);
    }
    #endregion

    public void AddLaptop(Player player)
    {
        players.Add(player.id, player);

        UdpClient new_conn = new UdpClient();
        new_conn.Connect(IPAddress.Parse(player.IP), Constants.P2P_PORT);
        laptopPeers.Add(player, new_conn);

        AudioHandler.AddPlayer(player.id);
    }

    public void AddCardboard(Player player)
    {
        players.Add(player.id, player);

        Client cardboard = new Client(player.IP, Constants.P2P_PORT);
        cardboard.ConnectToServer();
        cardboards.Add(player, cardboard);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
}
