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

    /// <summary>
    /// Message to syncronize games
    /// </summary>
    public class TickMessage : BaseMessage
    {
        public long Ticks;
        public float GameSpeed;

        public TickMessage(long ticks, float gameSpeed)
        {
            this.Ticks = ticks;
            this.GameSpeed = gameSpeed;
        }
    }
}
