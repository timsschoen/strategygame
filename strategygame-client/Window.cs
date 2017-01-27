﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using strategygame_common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_client
{
    abstract class Window
    {
        protected SpriteFont mFont;
        protected string mName;
        Texture2D mBackground;
        Texture2D mCloseSymbol;
        protected Rectangle mWindowRectangle;
        public bool IsOpen { get; set; } = false;

        bool mIsDragged = false;
        Point mDragStartRelativeToWindow;

        public virtual bool containsPoint(Point p)
        {
            return mWindowRectangle.Contains(p);
        }

        public virtual void handleMouseClick(Point p)
        {

        }
        
        private Rectangle getCloseSymbolRectangle()
        {
            return new Rectangle(mWindowRectangle.Right - 30, mWindowRectangle.Top + 10, 20, 20);            
        }

        public void setPosition(int X, int Y)
        {
            mWindowRectangle.X = X;
            mWindowRectangle.Y = Y;
        }
        
        public Window(string Name, ContentManager Content, int X, int Y)
        {
            this.mName = Name;
            
            mFont = Content.Load<SpriteFont>("Default");

            mWindowRectangle = new Rectangle(X, Y, 100, 100);

            mBackground = Content.Load<Texture2D>("UI/Windows/WindowBG");
            mCloseSymbol = Content.Load<Texture2D>("Ui/Windows/WindowX");
        }

        public virtual void draw(SpriteBatch spriteBatch, float Layer)
        {
            if (!IsOpen)
                return;

            spriteBatch.Draw(mBackground, mWindowRectangle, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, Layer);
            spriteBatch.Draw(mCloseSymbol, getCloseSymbolRectangle(), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, Layer + 0.01f);
            spriteBatch.DrawString(mFont, mName, new Vector2(mWindowRectangle.Left + 20, mWindowRectangle.Top + 10), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, Layer+0.01f);
        }

        public virtual void update()
        {
            if (!IsOpen)
                return;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Point MousePoint = Mouse.GetState().Position;
                if (!mIsDragged)
                {
                    if (mWindowRectangle.Contains(MousePoint))
                    {
                        if (getCloseSymbolRectangle().Contains(MousePoint))
                            this.IsOpen = false;

                        if (MousePoint.Y - mWindowRectangle.Top <= 20)
                        {
                            mIsDragged = true;
                            mDragStartRelativeToWindow = new Point(MousePoint.X - mWindowRectangle.X, MousePoint.Y - mWindowRectangle.Y);
                        }
                    }
                }
                else
                {
                    mWindowRectangle.X = MousePoint.X - mDragStartRelativeToWindow.X;
                    mWindowRectangle.Y = MousePoint.Y - mDragStartRelativeToWindow.Y;
                }
            }
            else
            {
                if (mIsDragged)
                    mIsDragged = false;
            }
        }
    }
}
