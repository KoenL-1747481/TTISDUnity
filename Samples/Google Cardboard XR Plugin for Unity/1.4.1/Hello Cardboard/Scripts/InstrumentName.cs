using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class InstrumentName : RequestPacket
{
    public InstrumentName(string instrument)
    {
        this.instrument = instrument;
    }

    public string instrument;
}
