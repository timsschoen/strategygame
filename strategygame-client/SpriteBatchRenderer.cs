using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace strategygame_client
{
    class SpriteBatchRenderer : ISpriteRenderer
    {
        SpriteBatch mSpriteBatch;
        SpriteFont mFont;

        /// <summary>
        /// Constructs a neew SpriteBatchRenderer
        /// </summary>
        /// <param name="spriteBatch">SpriteBacth to use for rendering</param>
        /// <param name="defaultFont">Font to use for drawing strings</param>
        public SpriteBatchRenderer(SpriteBatch spriteBatch, SpriteFont defaultFont)
        {
            mSpriteBatch = spriteBatch;
            mFont = defaultFont;
        }

        public void Begin()
        {
            mSpriteBatch.Begin(SpriteSortMode.FrontToBack);
        }

        public void Draw(Texture2D texture, Rectangle drawPosition, float layerDepth)
        {
            mSpriteBatch.Draw(texture, drawPosition, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        public void DrawRectanglePrimitive(Rectangle rectangle, int borderWidth, Color color, bool filled, float layerDepth)
        {
            Texture2D texture = new Texture2D(mSpriteBatch.GraphicsDevice, 1, 1);
            Color[] colorArray = new Color[1] { color };
            texture.SetData<Color>(colorArray);

            if (filled)
            {
                Draw(texture, rectangle, layerDepth);
            }
            else
            {
                Draw(texture, new Rectangle(rectangle.Location, new Point(borderWidth, rectangle.Height)), layerDepth);
                Draw(texture, new Rectangle(rectangle.Location, new Point(rectangle.Width, borderWidth)), layerDepth);
                Draw(texture, new Rectangle(new Point(rectangle.Right - borderWidth, rectangle.Top), new Point(borderWidth, rectangle.Height)), layerDepth);
                Draw(texture, new Rectangle(new Point(rectangle.X, rectangle.Bottom - borderWidth), new Point(rectangle.Width, borderWidth)), layerDepth);
            }
        }

        public void DrawString(string stringToDraw, Vector2 drawPosition, Color color, float layerDepth)
        {
            mSpriteBatch.DrawString(mFont, stringToDraw, drawPosition, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth);
        }

        public void End()
        {
            mSpriteBatch.End();
        }

        public void DrawText(string text, Rectangle rectangle, Color textColor, float layerDepth)
        {
            float fontscale = 0.8f;

            //split into words, maximal rectangle.width long
            string[] words = text.Split(' ');

            List<string> lines = new List<string>();

            string currentLine = "";

            for(int i = 0; i < words.Length; i++)
            {
                if (mFont.MeasureString(currentLine + " " + words[i]).X * 0.8f > rectangle.Width)
                {
                    lines.Add(currentLine);
                    currentLine = "";
                }

                currentLine += " " + words[i];
            }

            lines.Add(currentLine);

            Vector2 lineSize = mFont.MeasureString(lines[0]);

            for (int i = 0; i < lines.Count; i++)
            {
                mSpriteBatch.DrawString(mFont, lines[i], new Vector2(rectangle.X, rectangle.Y + i * fontscale * lineSize.Y), textColor, 0f, Vector2.Zero, fontscale, SpriteEffects.None, layerDepth);
            }

        }
    }
}
