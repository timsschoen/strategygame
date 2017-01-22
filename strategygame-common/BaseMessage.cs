using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class BaseMessage : IMessage
    {
        public int ClientID { get; set; } = -1;
    }
}
