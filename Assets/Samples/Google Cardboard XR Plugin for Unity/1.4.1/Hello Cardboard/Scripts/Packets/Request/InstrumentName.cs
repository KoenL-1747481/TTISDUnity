using Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class InstrumentName : RequestPacket
{
    public InstrumentName(int name)
    {
        this.name = name;
    }

    public int name;
}

