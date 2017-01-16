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

        private int TileSize;

        public Camera(int Tilesize)
        {
            TopLeftCoordinates = new Vector2();
            Zoom = 1f;
            TileSize = Tilesize;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState KS = Keyboard.GetState();

            if (KS.IsKeyDown(Keys.Right))
                TopLeftCoordinates.X++;
            if (KS.IsKeyDown(Keys.Left))
                TopLeftCoordinates.X--;
            if (KS.IsKeyDown(Keys.Up))
                TopLeftCoordinates.Y--;
            if (KS.IsKeyDown(Keys.Down))
                TopLeftCoordinates.Y++;
        }       
        
        public Vector2 getClickedHex(Point clickPos)
        {
            return new Vector2();
        }

        public Rectangle getRectangleToDrawCell(int x, int y)
        {
            Rectangle cellRect = new Rectangle();

            return cellRect;
        }

        public Rectangle getRectangleToDraw(int MapHeight, int MapWidth)
        {
            Rectangle rect = new Rectangle();

            rect.X = (int)Math.Floor(TopLeftCoordinates.X) - 1;
            rect.Y = (int)Math.Floor(TopLeftCoordinates.Y) - 1;

            rect.X = Math.Max(0, rect.X);
            rect.Y = Math.Max(0, rect.Y);

            rect.X = Math.Min(MapWidth, rect.X);
            rect.Y = Math.Min(MapHeight, rect.Y);

            rect.Width = (int)Math.Ceiling(GraphicsDeviceManager.DefaultBackBufferWidth / (TileSize * Zoom)) + 1;
            rect.Height = (int)Math.Ceiling(GraphicsDeviceManager.DefaultBackBufferHeight / (TileSize * Zoom)) + 1;

            return rect;
        } 
    }
}
