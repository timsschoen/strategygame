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
    class UIManager
    {
        public VillageWindow VillageWindow;

        public UIManager(ContentManager Content, IBuildingInformation BuildingData)
        {
            VillageWindow = new VillageWindow("Dorf", Content, 100, 100, BuildingData);
        }
        
        public bool ContainsPixel(Point p)
        {
            return VillageWindow.containsPoint(p) && VillageWindow.IsOpen;
        }

        public void Update(Dictionary<int, IEntity> Entities)
        {
            VillageWindow.update(Entities);
        }

        public void Draw(SpriteBatch spriteBatch, float Layer)
        {
            VillageWindow.draw(spriteBatch, Layer);
        }

        public void SelectVillage(Dictionary<int, IEntity> Entities, int selectedVillage)
        {
            VillageWindow.setVillage(selectedVillage);
        }

        public void handleMouseClick(Point position)
        {
            if (VillageWindow.containsPoint(position))
                VillageWindow.handleMouseClick(position);
        }
    }
}
