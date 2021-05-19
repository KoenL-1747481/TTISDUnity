using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TTISDProject
{   
    class Client_Old
    {
    /*    private static readonly string SERVER_IP = "127.0.0.1";
        private static readonly int SERVER_PORT = 25566;
        private static readonly int P2P_PORT = 25565;

        private static TcpConnection clientServer; // Client_Old-server connection

        private static List<UdpClient> peers = new List<UdpClient>(); // Peers to send data to
        private static Dictionary<string, int> peer_ids = new Dictionary<string, int>(); // Id's of peer ips

        private static UdpClient p2p_listener; // Listener that receives peer data
        private static bool listening;
        private static byte[] byte_buffer = new byte[1024 * 16];
        private static float[] float_buffer = new float[1024 * 16];
        private static object peer_lock = new object();

        public static void Connect()
        {
            Dispose();

            ConnectionResult result;
            clientServer = ConnectionFactory.CreateTcpConnection(SERVER_IP, SERVER_PORT, out result);
            if (result == ConnectionResult.Connected)
            {
                ConnectionEstablished();
            } else
            {
                Console.WriteLine("Failed to connect to server!");
                return;
            }

            p2p_listener = new UdpClient(P2P_PORT);
            listening = true;
            ThreadPool.QueueUserWorkItem(ListenerThread);
        }
        private static void ConnectionEstablished()
        {
            Console.WriteLine("Connection established with server.");
            clientServer.KeepAlive = true;
            // Register packet handlers
            clientServer.RegisterStaticPacketHandler<P2PInfo>(P2PInfoReceived);
            clientServer.RegisterStaticPacketHandler<LoopUpdate>(LoopUpdateRequestReceived);
        }

        public static void Dispose()
        {
            listening = false;
            clientServer?.Close(CloseReason.ClientClosed);
            p2p_listener?.Close();
            p2p_listener?.Dispose();
            peer_ids.Clear();
            lock (peer_lock)
            {
                foreach (UdpClient peer_conn in peers)
                {
                    peer_conn.Close();
                    peer_conn.Dispose();
                }
                peers.Clear();
            }
        }

        public static async void TryRecordLoop()
        {
            if (!LoopRecorder.IsRecording() && clientServer != null && clientServer.IsAlive) {
                RecordResponse response;
                try
                {
                    response = await clientServer.SendAsync<RecordResponse>(new RecordRequest());
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                if (response.OK)
                {
                    LoopRecorder.StartRecording(response.BPM, response.Bars);
                } else
                {
                    Console.WriteLine(response.Message);
                }
            }
        }

        public static async void TrySendLoop(float[] audio)
        {
            Console.WriteLine("Audio recorded. Size: " + audio.Length);
            if (clientServer != null)
            {
                SendLoopResponse response;
                try
                {
                    response = await clientServer.SendAsync<SendLoopResponse>(new SendLoopRequest(audio));
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                if (response.OK)
                {
                    Console.WriteLine("Loop successfully recorded and handled by server!");
                    AudioHandler.AddLoop(audio);
                } else
                {
                    Console.WriteLine(response.Message);
                }
            }
        }

        private static void ListenerThread(object state)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                while (listening)
                {
                    byte[] audio = p2p_listener.Receive(ref endpoint);
                    int peer_id;
                    if (peer_ids.TryGetValue(endpoint.Address.ToString(), out peer_id))
                    { // If peer is part of p2p network
                        // Convert to float array
                        Buffer.BlockCopy(audio, 0, float_buffer, 0, audio.Length);
                        // Play it
                        AudioHandler.PlayPlayerAudio(float_buffer, audio.Length / sizeof(float), peer_id);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void SendAudioToPeers(float[] audio, int count)
        {
            // Convert float data to bytes
            int amount_bytes = count * sizeof(float);
            Buffer.BlockCopy(audio, 0, byte_buffer, 0, amount_bytes);
            // Send to all peers
            lock (peer_lock)
            {
                foreach (var peer in peers)
                {
                    try
                    {
                        Console.WriteLine("Sending audio to: " + peer.Client.RemoteEndPoint.ToString());
                        peer.Send(byte_buffer, amount_bytes);
                    } catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        // This request contains a new loop that needs to be played
        private static void LoopUpdateRequestReceived(LoopUpdate req, Connection connection)
        {
            AudioHandler.AddLoop(req.Audio);
        }

        private static void P2PInfoReceived(P2PInfo data, Connection connection)
        {
            Console.WriteLine("Received p2p info from server!");
            // Print ips
            foreach (var ip in data.Peers)
                Console.WriteLine(ip);

            lock (peer_lock) // Lazy way to update connections
            {
                // Remove all existing connections
                foreach (UdpClient peer_conn in peers)
                {
                    peer_conn.Close();
                    peer_conn.Dispose();
                }
                peers.Clear();
                peer_ids.Clear();
                // Create connections with received peers
                int current_id = 1;
                foreach (string ip in data.Peers)
                {
                    UdpClient new_conn = new UdpClient();
                    new_conn.Connect(IPAddress.Parse(ip), P2P_PORT);
                    peers.Add(new_conn);
                    peer_ids.Add(ip, current_id++);
                }
            }
        }*/
    }
}
