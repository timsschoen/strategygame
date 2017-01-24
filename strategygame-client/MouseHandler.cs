using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_client
{
    class MouseHandler
    {
        public delegate void MouseClick(Point p);
        public delegate void Selection(Point A, Point B);

        public event MouseClick OnMouseClick;
        public event Selection OnSelection;

        private Texture2D CursorTexture;
        private Texture2D SelectedBGTexture;

        TimeSpan DragClickThreshhold = TimeSpan.FromSeconds(1);

        DateTime MousePressStarted;
        bool MouseWasPressed = false;
        Point MousePressStartPosition;
        
        bool isDragging;

        public MouseHandler(MouseClick OnMouseClick, Selection OnSelection, ContentManager Content)
        {
            this.OnMouseClick = OnMouseClick;
            this.OnSelection = OnSelection;
            this.CursorTexture = Content.Load<Texture2D>("UI/Mouse/Cursor");
            this.SelectedBGTexture = Content.Load<Texture2D>("UI/Mouse/SelectedBG");
        }

        public void Update()
        {
            MouseState MS = Mouse.GetState();
            if(MS.LeftButton == ButtonState.Pressed)
            {
                if(!MouseWasPressed)
                {
                    MouseWasPressed = true;
                    MousePressStarted = DateTime.Now;
                    MousePressStartPosition = MS.Position;
                }
                else
                {
                    if(DateTime.Now - MousePressStarted > DragClickThreshhold)
                    {
                        isDragging = true;
                    }
                }
            }
            else
            {
                if (MouseWasPressed)
                {
                    if(DateTime.Now - MousePressStarted > DragClickThreshhold)
                    {
                        //Dragging finished
                        if (OnSelection != null)
                            OnSelection(MousePressStartPosition, MS.Position);

                        isDragging = false;
                    }
                    else
                    {
                        //normal mouse click occured
                        if (OnMouseClick != null)
                            OnMouseClick(MS.Position);
                    }

                    MouseWasPressed = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, float LayerDepth)
        {
            spriteBatch.Draw(texture: CursorTexture, position: new Vector2(Mouse.GetState().X, Mouse.GetState().Y), layerDepth: LayerDepth);

            if(isDragging)
            {
                Point A = new Point(Math.Min(MousePressStartPosition.X, Mouse.GetState().Position.X), Math.Min(MousePressStartPosition.Y, Mouse.GetState().Position.Y));
                Point RectangleSize = new Point(Math.Max(MousePressStartPosition.X, Mouse.GetState().Position.X)-A.X, Math.Max(MousePressStartPosition.Y, Mouse.GetState().Position.Y)-A.Y);

                spriteBatch.Draw(texture: SelectedBGTexture, destinationRectangle: new Rectangle(A, RectangleSize), layerDepth: LayerDepth-0.001f);
            }
        }
    }
}
