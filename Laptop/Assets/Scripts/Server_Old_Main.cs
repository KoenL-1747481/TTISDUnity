using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TTISDProject
{
    class Server_Old_Main
    {
       /* public static readonly int MAX_PLAYERS = 4;
        private static int DEFAULT_BPM = 120;
        private static int DEFAULT_BARS = 4;

        private static ServerConnectionContainer server;
        private static readonly string EXTERNAL_IP = "94.110.227.197";
        private static readonly int PORT = 25566;
       */
        /* Session settings */
      /*  private static int BPM = DEFAULT_BPM;
        private static int Bars = DEFAULT_BARS;
        private static IPAddress RecordingAddress = null;
        private static Timer RecordTimeoutTimer = null;
        
        public static void Start()
        {
            Dispose();

            server = ConnectionFactory.CreateServerConnectionContainer(PORT);
            server.ConnectionEstablished += ConnectionEstablished;
            server.ConnectionLost += ConnectionLost;
            server.AllowUDPConnections = false;
            server.AllowBluetoothConnections = false;
            server.StartTCPListener();
        }


        public static void Dispose()
        {
            server?.Stop();
            server?.CloseConnections(CloseReason.ServerClosed);
            RecordTimeoutTimer?.Stop();
            RecordTimeoutTimer?.Close();
            RecordingAddress = null;
            BPM = DEFAULT_BPM;
            Bars = DEFAULT_BARS;
        }

        private static void ConnectionLost(Connection connection, ConnectionType type, CloseReason reason)
        {
            Console.WriteLine($"{server.Count} {type.ToString()} Connection lost {connection.IPRemoteEndPoint.Port}. Reason {reason.ToString()}");
            if (server.Count == MAX_PLAYERS - 1)
                server.StartTCPListener();

            UpdateP2P();
        }

        private static void ConnectionEstablished(Connection connection, ConnectionType type)
        {
            Console.WriteLine($"{server.Count} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");
            if (server.Count == MAX_PLAYERS)
                server.Stop();
            connection.KeepAlive = true;
            connection.RegisterStaticPacketHandler<RecordRequest>(OnRecordRequest);
            connection.RegisterStaticPacketHandler<SendLoopRequest>(OnSendLoopRequest);

            UpdateP2P();
        }

        private static void OnRecordRequest(RecordRequest req, Connection connection)
        {
            Console.WriteLine("RecordRequest received.");
            if (RecordingAddress != null)
            {
                Console.WriteLine("Someone is already recording!");
                connection.Send(new RecordResponse(req, false, "Someone is already recording!", BPM, Bars));
            }
            else
            {
                Console.WriteLine("Recording is allowed.");
                RecordingAddress = connection.IPRemoteEndPoint.Address;
                connection.Send(new RecordResponse(req, true, "", BPM, Bars));

                // If no SendLoopRequest after certain time, then timeout and reset current record request
                double clickInterval = (1.0 / (BPM / 60.0)) * 1000.0;
                double timeoutInterval = clickInterval * 4.0 * (Bars + 3.0);
                RecordTimeoutTimer = new Timer(timeoutInterval);
                RecordTimeoutTimer.Elapsed += (s, e_) =>
                {
                    RecordingAddress = null;
                    RecordTimeoutTimer.Stop();
                    RecordTimeoutTimer.Close();
                    Console.WriteLine("Record request timed out!");
                };
                RecordTimeoutTimer.Start();
            }
        }

        private static void OnSendLoopRequest(SendLoopRequest req, Connection connection)
        {
            Console.WriteLine("SendLoopRequest received.");
            Console.WriteLine("Loop size: " + req.Audio.Length);
            if (connection.IPRemoteEndPoint.Address == RecordingAddress)
            {
                RecordingAddress = null;
                RecordTimeoutTimer?.Stop();
                RecordTimeoutTimer?.Close();
                Console.WriteLine("IPs were equal.");
                connection.Send(new SendLoopResponse(req, true, ""));
                // Send loop to all other clients
                foreach (TcpConnection conn in server.TCP_Connections)
                {
                    if (conn.IPRemoteEndPoint.Address != connection.IPRemoteEndPoint.Address)
                    {
                        conn.Send(new LoopUpdate(req.Audio));
                    }
                }
            } else
            {
                Console.WriteLine("IPs were not equal.");
                connection.Send(new SendLoopResponse(req, false, "You didn't initiate a record request, or the request timed out."));
            }
        }
      
        private static void UpdateP2P()
        {
            // Get all connected clients
            var connections = server.TCP_Connections;
            List<string> ip_addresses = new List<string>();
            // Get all ip addresses of clients
            foreach (var conn in connections)
                ip_addresses.Add(conn.IPRemoteEndPoint.Address.ToString());
            // If server also is part of p2p network, replace local ip with external ip
            int i = ip_addresses.IndexOf("127.0.0.1");
            if (i != -1)
                ip_addresses[i] = EXTERNAL_IP;
            else
            {
                i = ip_addresses.IndexOf("192.168.0.1");
                if (i != -1)
                    ip_addresses[i] = EXTERNAL_IP;
            }
            // Send all ip addresses except their own to every client
            foreach (var conn in connections)
            {
                // Handle previous ip edit
                List<string> ip_addresses_to_send = ip_addresses.Where((v, ind) => v != conn.IPRemoteEndPoint.Address.ToString()).ToList();
                if (ip_addresses_to_send.Count == ip_addresses.Count)
                {
                    // Nothing changed so we are sending to local host
                    ip_addresses_to_send.Remove(EXTERNAL_IP);
                }
                conn.Send(new P2PInfo(ip_addresses_to_send));
            }
        }
      */
    } 
}
