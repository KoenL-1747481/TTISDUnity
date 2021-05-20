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
    public static string MyInstrument = "";  

    public static SessionManager instance;
    [SerializeField] SpawnPlayer spawnPlayer;

    public static Client clientServer;
    public static List<Player> cardboards = new List<Player>();

    private void OnApplicationQuit()
    {
        clientServer?.Disconnect();
    }

    public void Start()
    {
        clientServer = new Client(Constants.SERVER_IP, Constants.SERVER_PORT);
    }

    public void OnInstrumentChoose(string instrumentName)
    {
        MyInstrument = instrumentName;
        clientServer.ConnectToServer();
    }

    public void AddCardboard(Player player)
    {
        cardboards.Add(player);
        spawnPlayer.spawnPlayer(player.id, player.instrumentType);
    }

    public void HandleKinectData(int clientId, List<Quaternion> boneRotations)
    {
        spawnPlayer.updateAvatar(clientId, boneRotations);
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
