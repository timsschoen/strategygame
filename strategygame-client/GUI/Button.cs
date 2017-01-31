using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_client.GUI
{
    class Button
    {
        public event EventHandler OnClicked;

        string mText;
        Rectangle mPosition;

        public Button(ContentManager content, Rectangle position, string buttonText)
        {
            mText = buttonText;
            mPosition = position;
        }

        public void Draw(Vector2 offSetPosition, ISpriteRenderer spriteRenderer, long ticks, float layerDepth)
        {
            spriteRenderer.DrawRectanglePrimitive(new Rectangle(mPosition.X + (int)offSetPosition.X, mPosition.Y + (int)offSetPosition.Y, mPosition.Width, mPosition.Height), 3, Color.Black, false, layerDepth);
            spriteRenderer.DrawString(mText, offSetPosition + new Vector2(mPosition.X + 10, mPosition.Y + 10), Color.Black, layerDepth);
        }
        
        public void HandleMouseClick(Point clickPosition)
        {
            if (mPosition.Contains(clickPosition) && OnClicked != null)
                OnClicked(this, EventArgs.Empty);
        }        
    }
}
