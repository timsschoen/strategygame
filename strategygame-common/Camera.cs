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
        private Vector2 Scrolling;
        public float Zoom;
        public long LastScrollKeypress;

        private int ScreenWidth;
        private int ScreenHeight;
        private int MapWidth;
        private int MapHeight;
        private int TileSize;

        public void setScreenSize(int ScreenWidth, int ScreenHeight)
        {
            this.ScreenHeight = ScreenHeight;
            this.ScreenWidth = ScreenWidth;
        }

        public void setMapSize(int MapWidth, int MapHeight)
        {
            this.MapWidth = MapWidth;
            this.MapHeight = MapHeight;
        }

        public Camera(int Tilesize, int ScreenWidth, int ScreenHeight, int MapWidth, int MapHeight)
        {
            TopLeftCoordinates = new Vector2();
            Scrolling = new Vector2();
            Zoom = 2f;
            TileSize = Tilesize;

            this.ScreenHeight = ScreenHeight;
            this.ScreenWidth = ScreenWidth;
            this.MapWidth = MapWidth;
            this.MapHeight = MapHeight;
        }

        public Point Project(Vector2 Coordinates)
        {
            //TODO
            return new Point();
        }

        public void Update(GameTime gameTime)
        {
            TopLeftCoordinates += Scrolling;

            Vector2 BottomRight = TopLeftCoordinates + (new Vector2(ScreenWidth, ScreenHeight) / (TileSize * Zoom));

            //react to keyboard and mouse input
            MouseState MS = Mouse.GetState();
            KeyboardState KS = Keyboard.GetState();
            if (KS.IsKeyDown(Keys.Right) || ScreenWidth - MS.X < 50)
            {
                Scrolling.X+= 0.07f;
            }
            if (KS.IsKeyDown(Keys.Left) || MS.X < 50)
            {
                Scrolling.X -= 0.07f;
            }
            if (KS.IsKeyDown(Keys.Up) || MS.Y < 50)
            {
                Scrolling.Y -= 0.07f;
            }
            if (KS.IsKeyDown(Keys.Down) || ScreenHeight - MS.Y < 50)
            {
                Scrolling.Y += 0.07f;
            }
            
            //stop scrolling smoothly
            Scrolling /= 1.1f;

            //bounce back at the corners
            if (TopLeftCoordinates.X < -5)
                Scrolling.X -= (TopLeftCoordinates.X+5)/100;

            if (TopLeftCoordinates.Y < -5)
                Scrolling.Y -= (TopLeftCoordinates.Y+5) / 100;

            if (BottomRight.Y > MapHeight)
                Scrolling.Y -= (BottomRight.Y - MapHeight ) / 100;

            if (BottomRight.X > MapWidth+5)
                Scrolling.X -= (BottomRight.X - MapWidth - 5) / 100;
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

        public Rectangle getRectangleToDraw()
        {
            Rectangle rect = new Rectangle();

            rect.X = (int)Math.Floor(TopLeftCoordinates.X) - 1;
            rect.Y = (int)Math.Floor(TopLeftCoordinates.Y) - 1;

            rect.X = Math.Max(0, rect.X);
            rect.Y = Math.Max(0, rect.Y);

            rect.X = Math.Min(MapWidth-1, rect.X);
            rect.Y = Math.Min(MapHeight-1, rect.Y);

            rect.Width = (int)Math.Ceiling(ScreenWidth / (TileSize * Zoom)) + 3;
            rect.Height = (int)Math.Ceiling(ScreenHeight*1.34f / (TileSize * Zoom)) + 3;

            rect.Width = Math.Max(Math.Min(rect.Width, MapWidth - 1 - rect.X), 0);
            rect.Height = Math.Max(Math.Min(rect.Height, MapHeight - 1 - rect.Y), 0);

            return rect;
        } 
    }
}
