using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace strategygame_common
{
    public class Village : BaseEntity, IVillage
    {
        public static int MaxBuildingsInOneRow = 5;

        public Vector2 Position { get; set; }
        public bool isSelected { get; set; } = false;
        public List<Process> Processes { get; set; }
        public Queue<QueuedJob> ConstructionQueue { get; set; }
        public BuildingSlot[] Buildings { get; set; }
        public Dictionary<string, float> Attributes { get; set; }
        public float Hitboxsize { get; set; }
        public Point BuildingSlots { get; set; }
        public MapCellType CellType { get; set; }
        public IResources Resources { get; set; }

        new public void ClientInitialize()
        {
            Hitboxsize = 0.5f;
        }

        public Village(string Name, int Owner, Vector2 Position, Point BuildingSlots, MapCellType CellType, IResources Resources) : base(Name, Owner)
        {
            this.Position = Position;
            this.BuildingSlots = BuildingSlots;
            this.CellType = CellType;
            this.Resources = Resources;
            this.ConstructionQueue = new Queue<QueuedJob>();
            this.Processes = new List<Process>();
            Buildings = new BuildingSlot[BuildingSlots.X * BuildingSlots.Y];
        }        
    }

    public interface IVillage : IEntity, IResourceStore, ISelectableMapComponent
    {
        BuildingSlot[] Buildings { get; set; }
        List<Process> Processes { get; set; }
        Queue<QueuedJob> ConstructionQueue { get; set; }
        Dictionary<string, float> Attributes { get; set; }

        //X * Y Grid for buildings
        Point BuildingSlots { get; set; }
        MapCellType CellType { get; set; }
    }

    public class BuildingSlot
    {
        public int Level;
        public int Type;
        public bool Active;
    }

    public class BuildingMessage : BaseMessage
    {
        public int BuildingPosition;
        public int BuildingType;
        public int Level;
        public int VillageID;
    }    
}
