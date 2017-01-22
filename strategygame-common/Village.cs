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
        public Vector2 Position { get; set; }
        public bool isSelected { get; set; } = false;
        public List<ConstructionProcess> ParallelConstructions { get; set; }
        public List<QueuedJob> ConstructionQueue { get; set; }
        public List<Point> Buildings { get; set; }

        public Village(string Name, int Owner, Vector2 Position) : base(Name, Owner)
        {
            this.Position = Position;
        }        
    }

    public interface IVillage : IEntity, IResourceStore, ISelectableMapComponent
    {
        List<Point> Buildings { get; set; }
        List<ConstructionProcess> ParallelConstructions { get; set; }
        List<QueuedJob> ConstructionQueue { get; set; }
    }

    public class BuildingMessage : BaseMessage
    {
        public int BuildingPosition;
        public int BuildingType;
        public int Level;
    }
}
