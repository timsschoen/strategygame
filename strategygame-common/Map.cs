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
		MapCell[,] Cells;
		public int Height { get; }
		public int Width { get; }
		public bool editable { get; private set; }
                
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
                    Color MapColor = ColorArray[x + y * map.Width];
                    MapCellType CellType = MapCellType.Water;

                    if (MapColor.R > 200)
                        CellType = MapCellType.Flatland;
                    else if (MapColor.R > 150)
                        CellType = MapCellType.Hills;
                    else if (MapColor.R > 100)
                        CellType = MapCellType.Mountain;
                    else
                        CellType = MapCellType.Water;

                    MapCell mapCell = new MapCell(CellType);
                    map.Cells[x,y] = mapCell;
                }
            }

            return map;
        }

		public Map(int x, int y)
		{
			Cells = new MapCell[x,y];
			Height = y;
			Width = x;
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
			return Cells[x,y];
        }

		public void mapComplete()
		{
			editable = false;
		}

		public bool setCell(int x, int y, MapCell cell)
		{
			if (!editable) {
				return false;
			}

			Cells [x,y] = cell;
			return true;
		}
    }
}
