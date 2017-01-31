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
        public int BuildingSlot;
        public bool isUpgrading;

        public float interpolate(long ticks)
        {
            if (ticks < Start)
                return 0f;
            else if (ticks > End)
                return 1f;
            return (float)(ticks - Start) / (float)(End - Start);
        }
    }

    public class QueuedJob
    {
        public int BuildingSlot;
        public bool isUpgrading;
        public long Length;
    }
}
