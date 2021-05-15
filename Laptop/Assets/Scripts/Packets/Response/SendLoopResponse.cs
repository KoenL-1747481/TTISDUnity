/*using Network.Attributes;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTISDProject
{
    [PacketRequest(typeof(SendLoopRequest))]
    class SendLoopResponse : ResponsePacket
    {
        public SendLoopResponse(SendLoopRequest request, bool ok) : base(request)
        {
            OK = ok;
        }

        public bool OK { get; set; }
    }
}*/
