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
        public List<ConstructionProcess> ParallelConstructions { get; set; }
        public List<QueuedJob> ConstructionQueue { get; set; }
        public Point[] Buildings { get; set; }
        public float Hitboxsize { get; set; }
        public Point BuildingSlots { get; set; }
        public MapCellType CellType { get; set; }

        new public void ClientInitialize()
        {
            Hitboxsize = 0.5f;
        }

        public Village(string Name, int Owner, Vector2 Position, Point BuildingSlots, MapCellType CellType) : base(Name, Owner)
        {
            this.Position = Position;
            this.BuildingSlots = BuildingSlots;
            this.CellType = CellType;
            Buildings = new Point[BuildingSlots.X * BuildingSlots.Y];
        }        
    }

    public interface IVillage : IEntity, IResourceStore, ISelectableMapComponent
    {
        Point[] Buildings { get; set; }
        List<ConstructionProcess> ParallelConstructions { get; set; }
        List<QueuedJob> ConstructionQueue { get; set; }

        //X * Y Grid for buildings
        Point BuildingSlots { get; set; }
        MapCellType CellType { get; set; }
    }

    public class BuildingMessage : BaseMessage
    {
        public int BuildingPosition;
        public int BuildingType;
        public int Level;
    }
}
