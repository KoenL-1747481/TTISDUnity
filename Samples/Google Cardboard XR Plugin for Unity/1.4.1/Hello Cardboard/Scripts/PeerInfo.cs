using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class PeerInfo: RequestPacket
{
    public PeerInfo(string ip, string instrument)
    {
        this.ip = ip;
        this.instrument = instrument;
    }

    public string ip;
    public string instrument;
}
