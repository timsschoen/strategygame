﻿using Microsoft.Xna.Framework;
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

        public event MouseClick OnMapMouseClick;
        public event Selection OnSelection;

        private Texture2D mCursorTexture;
        private Texture2D mSelectedBGTexture;

        TimeSpan mDragClickThreshhold = TimeSpan.FromSeconds(1);
        
        DateTime mMousePressStarted;
        bool mMouseWasPressed = false;
        Point mMousePressStartPosition;
        
        bool mIsDragging;

        public MouseHandler(MouseClick OnMouseClick, Selection OnSelection, ContentManager Content)
        {
            this.OnMapMouseClick = OnMouseClick;
            this.OnSelection = OnSelection;
            this.mCursorTexture = Content.Load<Texture2D>("UI/Mouse/Cursor");
            this.mSelectedBGTexture = Content.Load<Texture2D>("UI/Mouse/SelectedBG");
        }

        public void update(UIManager UI)
        {
            MouseState MS = Mouse.GetState();

            if(MS.LeftButton == ButtonState.Pressed)
            {
                if(!mMouseWasPressed)
                {
                    mMouseWasPressed = true;
                    mMousePressStarted = DateTime.Now;
                    mMousePressStartPosition = MS.Position;
                }
                else
                {
                    if(DateTime.Now - mMousePressStarted > mDragClickThreshhold && !UI.ContainsPixel(Mouse.GetState().Position))
                    {
                        mIsDragging = true;
                    }
                }
            }
            else
            {
                if (mMouseWasPressed)
                {
                    if(mIsDragging)
                    {
                        //Dragging finished
                        if (OnSelection != null)
                            OnSelection(mMousePressStartPosition, MS.Position);

                        mIsDragging = false;
                    }
                    else
                    {
                        if (UI.ContainsPixel(MS.Position))
                        {
                            UI.handleMouseClick(MS.Position);
                        }
                        else
                        {
                            //normal mouse click occured
                            if (OnMapMouseClick != null)
                                OnMapMouseClick(MS.Position);
                        }
                    }

                    mMouseWasPressed = false;
                }
            }
        }

        public void draw(SpriteBatch spriteBatch, float LayerDepth)
        {
            spriteBatch.Draw(texture: mCursorTexture, position: new Vector2(Mouse.GetState().X, Mouse.GetState().Y), layerDepth: LayerDepth);

            if(mIsDragging)
            {
                Point A = new Point(Math.Min(mMousePressStartPosition.X, Mouse.GetState().Position.X), Math.Min(mMousePressStartPosition.Y, Mouse.GetState().Position.Y));
                Point RectangleSize = new Point(Math.Max(mMousePressStartPosition.X, Mouse.GetState().Position.X)-A.X, Math.Max(mMousePressStartPosition.Y, Mouse.GetState().Position.Y)-A.Y);

                spriteBatch.Draw(texture: mSelectedBGTexture, destinationRectangle: new Rectangle(A, RectangleSize), layerDepth: LayerDepth-0.001f);
            }
        }
    }
}
