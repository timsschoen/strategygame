using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_client
{
    class WindowManager
    {
        public VillageWindow VillageWindow;

        public WindowManager(ContentManager Content)
        {
            VillageWindow = new VillageWindow("Dorf", Content, 100, 100);
        }

        public void addWindow(Window window)
        {

        }

        public void Update()
        {
            VillageWindow.Update();
        }

        public void Draw(SpriteBatch spriteBatch, float Layer)
        {
            VillageWindow.Draw(spriteBatch, Layer);
        }
    }
}
