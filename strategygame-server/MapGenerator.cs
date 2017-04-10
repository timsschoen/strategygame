using System;
using strategygame_common;

namespace strategygame_server
{
	public class MapGenerator
	{
		Map map { get;}

		public MapGenerator (int x, int y)
		{
			map = new Map (x, y);
		}

		public void generateMap ()
		{
			for (int i = 0; i < map.Width; i++) {
				for (int j = 0; j < map.Height; j++) {
					Random random = new Random ();
					Resources res = new Resources ();
					for (int k = 0; k < 5; k++) {
						res.SetResourceCount (k, random.Next (1000));
					}
					MapCell newCell = new MapCell ((MapCellType)random.Next(10), res);
					map.setCell (i, j, newCell);
				}
			}
		}
	}
}

