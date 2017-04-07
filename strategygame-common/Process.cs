using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class ConstructionProcess : Process
    {
        public int BuildingSlot;
        public bool isUpgrading;
    }

    public class ProductionProcess : Process
    {
        public bool Repeating;
        public int OwnerBuildingSlot;
        public IResources In;
        public IResources Out;
        public int Length;

        public static ProductionProcess FromProduction(ProductionEffect production, long currentTicks, int buildingSlot)
        {
            ProductionProcess process = new ProductionProcess();
            process.Start = currentTicks;
            process.In = production.In;
            process.Out = production.Out;
            process.Length = production.Length;
            process.OwnerBuildingSlot = buildingSlot;
            process.Repeating = true;

            return process;
        }
    }

    public class Process
    {
        public long Start;
        public long End;

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
