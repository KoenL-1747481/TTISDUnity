using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ServerClient
{
    public int id;
    public TCP tcp;
    public UDP udp;
    
    public Player player;

    public ServerClient(int _clientId)
    {
        id = _clientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public class TCP
    {
        public TcpClient socket;

        private readonly int id;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public TCP(int _id)
        {
            id = _id;
        }

        /// <summary>Initializes the newly connected client's TCP-related info.</summary>
        /// <param name="_socket">The TcpClient instance of the newly connected client.</param>
        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = Constants.DATA_BUFFER_SIZE;
            socket.SendBufferSize = Constants.DATA_BUFFER_SIZE;

            stream = socket.GetStream();

            receivedData = new Packet();
            receiveBuffer = new byte[Constants.DATA_BUFFER_SIZE];

            stream.BeginRead(receiveBuffer, 0, Constants.DATA_BUFFER_SIZE, ReceiveCallback, null);

            ServerSend.Welcome(id, "Welcome to the server!");
        }

        /// <summary>Sends data to the client via TCP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); // Send data to appropriate client
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to player {id} via TCP: {_ex}");
            }
        }

        /// <summary>Reads incoming data from the stream.</summary>
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Server.clients[id].Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data)); // Reset receivedData if all data was handled
                stream.BeginRead(receiveBuffer, 0, Constants.DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving TCP data: {_ex}");
                Server.clients[id].Disconnect();
            }
        }

        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="_data">The recieved data.</param>
        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                // If client's received data contains a packet
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    // If packet contains no data
                    return true; // Reset receivedData instance to allow it to be reused
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
                    }
                });

                _packetLength = 0; // Reset packet length
                if (receivedData.UnreadLength() >= 4)
                {
                    // If client's received data contains another packet
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        // If packet contains no data
                        return true; // Reset receivedData instance to allow it to be reused
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true; // Reset receivedData instance to allow it to be reused
            }

            return false;
        }

        /// <summary>Closes and cleans up the TCP connection.</summary>
        public void Disconnect()
        {
            socket?.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public IPEndPoint endPoint;

        private int id;

        public UDP(int _id)
        {
            id = _id;
        }

        /// <summary>Initializes the newly connected client's UDP-related info.</summary>
        /// <param name="_endPoint">The IPEndPoint instance of the newly connected client.</param>
        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
        }

        /// <summary>Sends data to the client via UDP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendData(Packet _packet)
        {
            Server.SendUDPData(endPoint, _packet);
        }

        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="_packetData">The packet containing the recieved data.</param>
        public void HandleData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
                }
            });
        }

        /// <summary>Cleans up the UDP connection.</summary>
        public void Disconnect()
        {
            endPoint = null;
        }
    }

    /// <summary>Sends the client into the game and informs other clients of the new player.</summary>
    /// <param name="_playerName">The username of the new player.</param>
    public void SendIntoSession(string _playerName, string _instrumentType = null)
    {
        string player_ip = ((IPEndPoint)tcp.socket.Client.RemoteEndPoint).Address.ToString();
        if (player_ip == "192.168.0.1" || player_ip == "127.0.0.1")
            player_ip = Constants.SERVER_IP;
        Debug.Log("Player IP: " + player_ip);

        player = new Player(id, _playerName, player_ip, _instrumentType);

        // Send all players (except himself) to the new player if player is non-cardboard
        // Send all cardboards (except himself) to the new player if player is cardboard
        foreach (ServerClient _client in Server.clients.Values)
        {
            if (_client.player != null && _client.id != id)
            {
                if (_client.player.instrumentType != null) // Other player is cardboard
                    ServerSend.AddCardboard(id, _client.player); // Always add cardboard
                else // Other player is laptop
                { 
                    if (_instrumentType == null) // You are a laptop
                        ServerSend.AddLaptop(id, _client.player); // Laptops get laptops
                }
            }
        }

        // Send the new player to all non-cardboard players (except himself)
        // Send the new player to all cardboard players (except himself) if player is cardboard
        foreach (ServerClient _client in Server.clients.Values)
        {
            if (_client.player != null && _client.id != id)
            {
                if (_client.player.instrumentType == null) // Other player is laptop
                {
                    if (_instrumentType == null)
                        ServerSend.AddLaptop(_client.id, player);
                    else
                        ServerSend.AddCardboard(_client.id, player);
                } else // Other player is cardboard
                {
                    if (_instrumentType != null)
                        ServerSend.AddCardboard(_client.id, player);
                }
            }
        }
    }

    /// <summary>Disconnects the client and stops all network traffic.</summary>
    public void Disconnect()
    {
        Debug.Log($"{tcp.socket?.Client?.RemoteEndPoint} has disconnected.");

        tcp?.Disconnect();
        udp?.Disconnect();
    }
}
