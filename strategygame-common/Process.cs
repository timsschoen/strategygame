using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class ConstructionProcess
    {
        public long Start;
        public long End;
        public int Updating;
        public bool isUpgrading;
    }

    public class QueuedJob
    {
        public int Updating;
        public bool isUpgrading;
        public long Length;
    }
}
