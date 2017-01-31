using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace strategygame_common
{
    public class Map
    {
        MapCell[] Cells;
        public int Height { get; private set; }
        public int Width { get; private set; }
                
        public static Map LoadFromFolder(GraphicsDevice device, string PathToFolder)
        {
            MonoGame.Utilities.Png.PngReader Reader = new MonoGame.Utilities.Png.PngReader();

            Texture2D BaseTypePNG = Reader.Read(File.Open(PathToFolder + "/Map.png", System.IO.FileMode.Open), device);
                        
            Map map = new Map(BaseTypePNG.Height, BaseTypePNG.Width);

            Color[] ColorArray = new Color[map.Height * map.Width];
            BaseTypePNG.GetData<Color>(ColorArray);

            for(int x = 0; x < map.Width; x++)
            {
                for(int y = 0; y < map.Width; y++)
                {
                    MapCell mapCell = new MapCell(ColorArray[x+y*map.Width]);
                    map.Cells[x + y * map.Width] = mapCell;
                }
            }

            return map;
        }

        public Map(int Height, int Width)
        {
            this.Height = Height;
            this.Width = Width;
            Cells = new MapCell[Height * Width];
        }

        /// <summary>
        /// Returns the mapCell at the specified coordinates, or null, if the coordinates are out of bounds
        /// </summary>
        /// <param name="x">x-Coordinate of the MapCell (max: Map.Width-1, min: 0)</param>
        /// <param name="y">y-Coordinate of the MapCell (max: Map.Height-1, min: 0)</param>
        /// <returns></returns>
        public MapCell getMapCellAt(int x, int y)
        {
            if((x < 0) || (y < 0) || (y > Height-1) || (x > Width-1))
            {
                return null;
            }

            return Cells[x + y * Width];
        }
    }
}
