using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class Camera
    {
        public Vector2 TopLeftCoordinates;
        public float Zoom;
        public long LastScrollKeypress;

        private int TileSize;

        public Camera(int Tilesize)
        {
            TopLeftCoordinates = new Vector2();
            Zoom = 2f;
            TileSize = Tilesize;
        }

        public void Update(GameTime gameTime)
        {

            if (gameTime.TotalGameTime.TotalMilliseconds - LastScrollKeypress > 500)
            {
                KeyboardState KS = Keyboard.GetState();

                if (KS.IsKeyDown(Keys.Right))
                {
                    TopLeftCoordinates.X++;
                    LastScrollKeypress = (long)gameTime.TotalGameTime.TotalMilliseconds;
                }
                if (KS.IsKeyDown(Keys.Left))
                {
                    TopLeftCoordinates.X--;
                    LastScrollKeypress = (long)gameTime.TotalGameTime.TotalMilliseconds;
                }
                if (KS.IsKeyDown(Keys.Up))
                {
                    TopLeftCoordinates.Y--;
                    LastScrollKeypress = (long)gameTime.TotalGameTime.TotalMilliseconds;
                }
                if (KS.IsKeyDown(Keys.Down))
                {
                    TopLeftCoordinates.Y++;
                    LastScrollKeypress = (long)gameTime.TotalGameTime.TotalMilliseconds;
                }
                
            }
        }       
        
        public Vector2 getClickedHex(Point clickPos)
        {
            return new Vector2();
        }

        public Rectangle getRectangleToDrawCell(int x, int y)
        {
            Rectangle cellRect = new Rectangle();

            cellRect.X = (int)((x - TopLeftCoordinates.X) * TileSize*Zoom);
            cellRect.Y = (int)((0.75f*(y - TopLeftCoordinates.Y)) * TileSize*Zoom);

            if (y % 2 != 0)
                cellRect.X += (int)(0.5f*TileSize * Zoom);

            cellRect.Width = (int)(TileSize * Zoom);
            cellRect.Height = (int)(TileSize * Zoom);
            
            return cellRect;
        }

        public Rectangle getRectangleToDraw(int MapHeight, int MapWidth, int ScreenHeight, int ScreenWidth)
        {
            Rectangle rect = new Rectangle();

            rect.X = (int)Math.Floor(TopLeftCoordinates.X) - 1;
            rect.Y = (int)Math.Floor(TopLeftCoordinates.Y) - 1;

            rect.X = Math.Max(0, rect.X);
            rect.Y = Math.Max(0, rect.Y);

            rect.X = Math.Min(MapWidth, rect.X);
            rect.Y = Math.Min(MapHeight, rect.Y);

            rect.Width = (int)Math.Ceiling(ScreenWidth / (TileSize * Zoom)) + 1;
            rect.Height = (int)Math.Ceiling(ScreenHeight*1.34f / (TileSize * Zoom)) + 1;

            return rect;
        } 
    }
}
