using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace strategiespiel2
{
    interface IEntity
    {
        void Draw(SpriteBatch spriteBatch);
        void Update(GameTime gameTime);
    }
}
