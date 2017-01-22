using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using strategygame_common;

namespace strategygame_client
{
    class VillageWindow : Window
    {
        public VillageWindow(string Name, ContentManager Content, int X, int Y) : base(Name, Content, X, Y)
        {
            WindowRectangle.Width = 300;
            WindowRectangle.Height = 500;
        }

        public void setVillage(IVillage Village)
        {
            this.Name = Village.Name;
        }

        public override void Draw(SpriteBatch spriteBatch, float Layer)
        {
            base.Draw(spriteBatch, Layer);
        }
    }
}
