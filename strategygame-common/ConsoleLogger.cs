using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class ConsoleLogger : ILogger
    {
        public void Log(LogPriority Priority, string TAG, string Content)
        {
            string priority = "Normal   ";
            if (Priority == LogPriority.Important)
                priority =    "Important";
            else if (Priority == LogPriority.Verbose)
                priority =    "Verbose  ";

            Console.Out.WriteLine(priority + " | " + TAG + " | " + Content);
        }
    }
}
