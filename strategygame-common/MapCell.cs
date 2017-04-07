using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace strategygame_common
{
    public class MapCell
    {
        public bool IsWater { get; private set; }
        public float Laufgeschwindigkeit { get; private set; }
        public float Fruchtbarkeit { get; private set; }
        public Resources ResourceFactors;
        public MapCellType CellType;

		public MapCell(MapCellType type, Resources resources)
        {
			CellType = type;
			ResourceFactors = resources;
        }
        
    }
    
    public enum MapCellType
    {
        Mountain,
        Water,
        Flatland,
        Plateau,
        Swamp,
        Hills,
        Forest,
        Ice,
        Jungle,
        Tundra
    }
}
