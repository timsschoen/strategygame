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
        public bool isWater { get; private set; }
        public float Laufgeschwindigkeit { get; private set; }
        public float Fruchtbarkeit { get; private set; }
        public Vector4 AvailableResources;
        public MapCellType CellType;
        public MapCellSegment[] Segments { get; private set; }

        public MapCell(Color MapColor)
        {
            Segments = new MapCellSegment[7];

            if (MapColor.R > 200)
                CellType = MapCellType.Flatland;
            else if (MapColor.R > 150)
                CellType = MapCellType.Hills;
            else if (MapColor.R > 100)
                CellType = MapCellType.Mountain;
            else
                CellType = MapCellType.Water;
        }
        
    }

    public class MapCellSegment
    {

    }

    public enum MapCellType
    {
        Mountain,
        Water,
        Flatland,
        Swamp,
        Hills
    }
}
