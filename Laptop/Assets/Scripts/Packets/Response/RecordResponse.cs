/*using Network.Attributes;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTISDProject
{
    [PacketRequest(typeof(RecordRequest))]
    class RecordResponse : ResponsePacket
    {
        public RecordResponse(RecordRequest request, bool ok, int bpm, int bars) : base(request)
        {
            OK = ok;
            BPM = bpm;
            Bars = bars;
        }
        public bool OK { get; set; }
        public int BPM { get; set; }
        public int Bars { get; set; }
    }
}*/
