using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        bool DebugMode = false;
        bool F2WasPressed = false;

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

            if(Keyboard.GetState().IsKeyDown(Keys.F2) && !F2WasPressed)
            {
                F2WasPressed = true;
                DebugMode = !DebugMode;
            }
            else if(Keyboard.GetState().IsKeyUp(Keys.F2))
            {
                F2WasPressed = false;
            }

        }

        /// <summary>
        /// Draw the UI to the given SpriteBatch
        /// </summary>
        /// <param name="spriteRenderer"></param>
        /// <param name="Layer"></param>
        public void Draw(ISpriteRenderer spriteRenderer, float Layer, long ticks)
        {
            VillageWindow.Draw(spriteRenderer, Layer, ticks);

            if(DebugMode)
            {
                spriteRenderer.DrawString("Ticks: " + ticks, new Vector2(20, 20), Color.Red, 0.9f);
            }
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
