using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_client.GUI
{
    interface IGUIElement
    {
        void HandleMouseClick(Point clickPosition);        
        void Draw(Vector2 offSetPosition, ISpriteRenderer spriteRenderer, long ticks, float layerDepth);
    }
}
