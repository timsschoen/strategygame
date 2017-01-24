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

        private Matrix Projection;

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

        //http://gamedev.stackexchange.com/a/59450/42131
        public Rectangle VisibleArea
        {
            get
            {
                var inverseViewMatrix = Matrix.Invert(Projection);
                var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
                var tr = Vector2.Transform(new Vector2(ScreenWidth, 0), inverseViewMatrix);
                var bl = Vector2.Transform(new Vector2(0, ScreenHeight), inverseViewMatrix);
                var br = Vector2.Transform(new Vector2(ScreenWidth, ScreenHeight), inverseViewMatrix);
                var min = new Vector2(
                    MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                    MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
                var max = new Vector2(
                    MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                    MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
                return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
            }
        }

        public Camera(int Tilesize, int ScreenWidth, int ScreenHeight, int MapWidth, int MapHeight)
        {
            TopLeftCoordinates = new Vector2();
            Scrolling = new Vector2();
            Zoom = 1f;
            TileSize = Tilesize;

            this.ScreenHeight = ScreenHeight;
            this.ScreenWidth = ScreenWidth;
            this.MapWidth = MapWidth;
            this.MapHeight = MapHeight;

            Projection = Matrix.CreateTranslation(TopLeftCoordinates.X, TopLeftCoordinates.Y, 0) * Matrix.CreateScale(Zoom * TileSize, 0.75f * Zoom * TileSize, 1);

        }

        public Point Project(Vector2 Coordinates)
        {
            return new Point();
        }

        public void Update()
        {
            TopLeftCoordinates += Scrolling;

            Vector2 BottomRight = TopLeftCoordinates + (new Vector2(ScreenWidth, ScreenHeight) / (TileSize * Zoom));

            //react to keyboard and mouse input
            MouseState MS = Mouse.GetState();
            KeyboardState KS = Keyboard.GetState();
            if (KS.IsKeyDown(Keys.Right) || ScreenWidth - MS.X < 50)
            {
                Scrolling.X -= 0.07f;
            }
            if (KS.IsKeyDown(Keys.Left) || MS.X < 50)
            {
                Scrolling.X += 0.07f;
            }
            if (KS.IsKeyDown(Keys.Up) || MS.Y < 50)
            {
                Scrolling.Y += 0.07f;
            }
            if (KS.IsKeyDown(Keys.Down) || ScreenHeight - MS.Y < 50)
            {
                Scrolling.Y -= 0.07f;
            }
            
            //stop scrolling smoothly
            Scrolling /= 1.1f;

            //bounce back at the corners
            Vector2 OffSetTopLeft = Unproject(Vector2.Zero)+new Vector2(5,5);
            Vector2 OffSetBottomRight = Unproject(new Vector2(ScreenWidth, ScreenHeight))-new Vector2(MapWidth+5, MapHeight+5);            

            OffSetTopLeft.X = (OffSetTopLeft.X > 0) ? 0 : OffSetTopLeft.X;
            OffSetTopLeft.Y = (OffSetTopLeft.Y > 0) ? 0 : OffSetTopLeft.Y;
            OffSetBottomRight.X = (OffSetBottomRight.X < 0) ? 0 : OffSetBottomRight.X;
            OffSetBottomRight.Y = (OffSetBottomRight.Y < 0) ? 0 : OffSetBottomRight.Y;

            Scrolling += 0.02f * (OffSetTopLeft + OffSetBottomRight);

            Projection = Matrix.CreateTranslation(TopLeftCoordinates.X, TopLeftCoordinates.Y, 0) * Matrix.CreateScale(Zoom*TileSize, 0.75f*Zoom*TileSize, 1);
        }

        public Rectangle getRectangleToDraw()
        {
            Rectangle Result = VisibleArea;

            Result.X -= 3;
            Result.Y -= 3;
            Result.Width += 6;
            Result.Height += 6;

            Result.X = Math.Max(0, Result.X);
            Result.Y = Math.Max(0, Result.Y);
            Result.Width = Math.Min(MapWidth-Result.X-1, Result.Right);
            Result.Height = Math.Min(MapHeight -Result.Y - 1, Result.Bottom);

            return Result;
        }

        public Vector2 Unproject(Vector2 clickPos)
        {        
            return Vector2.Transform(clickPos, Matrix.Invert(Projection));
        }

        public Point getClickedHex(Vector2 clickPos)
        {
            Vector2 Unprojected = Unproject(clickPos);
            Point toReturn = new Point((int)Unprojected.X, (int)Unprojected.Y);
            if (toReturn.X % 2 == 1)
                toReturn.Y = (int)(Unprojected.X + 0.5f);

            return toReturn;
        }

        public Rectangle getRectangleToDrawCell(int x, int y)
        {
            Rectangle cellRect = new Rectangle();

            Vector2 CellBasePoint = new Vector2(x, y);
            CellBasePoint = Vector2.Transform(CellBasePoint, Projection);

            cellRect.X = (int)(CellBasePoint.X);
            cellRect.Y = (int)(CellBasePoint.Y);

            if (y % 2 != 0)
                cellRect.X += (int)(0.5f*TileSize * Zoom);

            cellRect.Width = (int)(TileSize * Zoom);
            cellRect.Height = (int)(TileSize * Zoom);
            
            return cellRect;
        }        
    }
}
