using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_client
{
    class Window
    {
        Texture2D TitleBar;
        Texture2D Bottom;
        Texture2D Right;
        Texture2D Left;
        SpriteFont Font;

        public Window(string Name, Texture2D TitleBar, Texture2D Bottom, Texture2D Right, Texture2D Left, SpriteFont Font)
        {
            this.TitleBar = TitleBar;
            this.Bottom = Bottom;
            this.Right = Right;
            this.Left = Left;
        }

        public void Draw(SpriteBatch spriteBatch, float Layer)
        {

        }
    }
}
