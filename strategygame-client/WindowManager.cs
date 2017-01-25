using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using strategygame_common;
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

        public WindowManager(ContentManager Content, IBuildingInformation BuildingData)
        {
            VillageWindow = new VillageWindow("Dorf", Content, 100, 100, BuildingData);
        }

        public void addWindow(Window window)
        {

        }

        public bool ContainsPixel(Point p)
        {
            return VillageWindow.containsPoint(p);
        }

        public void Update()
        {
            VillageWindow.Update();
        }

        public void Draw(Dictionary<int, IEntity> Entities, SpriteBatch spriteBatch, float Layer)
        {
            VillageWindow.Draw(Entities, spriteBatch, Layer);
        }
    }
}
