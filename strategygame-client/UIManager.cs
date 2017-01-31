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

        public UIManager(ContentManager Content, IBuildingInformation BuildingData, GraphicsDevice graphicsDevice, INetworkSender networkSender)
        {
            VillageWindow = new VillageWindow("Dorf", Content, 100, 100, BuildingData, graphicsDevice, networkSender);
        }
        
        public bool ContainsScreenPoint(Point p)
        {
            return VillageWindow.ContainsScreenPoint(p) && VillageWindow.IsOpen;
        }

        public void Update(Dictionary<int, IEntity> Entities)
        {
            VillageWindow.Update(Entities);
        }

        /// <summary>
        /// Draw the UI to the given SpriteBatch
        /// </summary>
        /// <param name="spriteRenderer"></param>
        /// <param name="Layer"></param>
        public void Draw(ISpriteRenderer spriteRenderer, float Layer)
        {
            VillageWindow.Draw(spriteRenderer, Layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Entities"></param>
        /// <param name="selectedVillage"></param>
        public void SelectVillage(int selectedVillage)
        {
            VillageWindow.setVillage(selectedVillage);
        }

        public void HandleMouseClick(Point position)
        {
            if (VillageWindow.ContainsScreenPoint(position))
                VillageWindow.HandleMouseClick(position);
        }
    }
}
