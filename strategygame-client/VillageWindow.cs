using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace strategygame_client
{
    class VillageWindow : Window
    {
        public VillageWindow(string Name, ContentManager Content, int X, int Y) : base(Name, Content, X, Y)
        {
            
        }

        public void setVillage(IVillage Village)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, float Layer)
        {
            base.Draw(spriteBatch, Layer);
        }
    }

    interface IVillage
    {

    }
}
