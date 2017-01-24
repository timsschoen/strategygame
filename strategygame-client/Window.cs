using Microsoft.Xna.Framework;
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
        SpriteFont Font;
        protected string Name;
        Texture2D Background;
        Texture2D CloseSymbol;
        protected Rectangle WindowRectangle;

        public virtual bool containsPoint(Point p)
        {
            return WindowRectangle.Contains(p);
        }

        private Rectangle CloseSymbolRectangle
        {
            get
            {
                return new Rectangle(WindowRectangle.Right - 30, WindowRectangle.Top + 10, 20, 20);
            }
        }

        public void setPosition(int X, int Y)
        {
            WindowRectangle.X = X;
            WindowRectangle.Y = Y;
        }

        protected bool isOpen = true;

        bool isDragged = false;
        Point DragStartRelativeToWindow;

        public Window(string Name, ContentManager Content, int X, int Y)
        {
            this.Name = Name;
            
            Font = Content.Load<SpriteFont>("Font");

            WindowRectangle = new Rectangle(X, Y, 100, 100);

            Background = Content.Load<Texture2D>("UI/Windows/WindowBG");
            CloseSymbol = Content.Load<Texture2D>("Ui/Windows/WindowX");
        }

        public virtual void Draw(SpriteBatch spriteBatch, float Layer)
        {
            if (!isOpen)
                return;

            spriteBatch.Draw(Background, WindowRectangle, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, Layer);
            spriteBatch.Draw(CloseSymbol, CloseSymbolRectangle , null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, Layer + 0.01f);
            spriteBatch.DrawString(Font, Name, new Vector2(WindowRectangle.Left + 20, WindowRectangle.Top + 10), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, Layer+0.01f);
        }

        public virtual void Update()
        {
            if (!isOpen)
                return;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Point MousePoint = Mouse.GetState().Position;
                if (!isDragged)
                {
                    if (WindowRectangle.Contains(MousePoint))
                    {
                        if (CloseSymbolRectangle.Contains(MousePoint))
                            this.isOpen = false;

                        if (MousePoint.Y - WindowRectangle.Top <= 20)
                        {
                            isDragged = true;
                            DragStartRelativeToWindow = new Point(MousePoint.X - WindowRectangle.X, MousePoint.Y - WindowRectangle.Y);
                        }
                    }
                }
                else
                {
                    WindowRectangle.X = MousePoint.X - DragStartRelativeToWindow.X;
                    WindowRectangle.Y = MousePoint.Y - DragStartRelativeToWindow.Y;
                }
            }
            else
            {
                if (isDragged)
                    isDragged = false;
            }
        }
    }
}
