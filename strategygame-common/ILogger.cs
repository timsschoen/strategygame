using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public interface ILogger
    {
        void Log(LogPriority Priority, string TAG, string Content);
    }

    public enum LogPriority
    {
        Normal,
        Verbose,
        Important
    }
        
}
