using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Constants
{
    public const int MAX_PLAYERS = 8; // 4 Cardboard, 4 Laptop
    public const int SERVER_PORT = 25566;
    public const int P2P_PORT = 25565;
    public const int CARDBOARD_PORT = 25567;
    public const string SERVER_IP = "141.135.129.110";
    public const int DATA_BUFFER_SIZE = 4096;

    private static readonly string LOCAL_IP_KOEN = "192.168.0.68";
    private static readonly string LOCAL_IP_JEFFREY = "192.168.0.212";
    private static readonly string LOCAL_IP_SEM = "192.168.0.186";
    private static readonly string SERVER_IP_KOEN = "94.110.227.197";
    private static readonly string SERVER_IP_JEFFREY = "84.193.179.2";
    private static readonly string SERVER_IP_SEM = "141.135.129.110";
}
