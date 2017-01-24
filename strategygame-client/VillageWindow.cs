using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using strategygame_common;
using Microsoft.Xna.Framework;

namespace strategygame_client
{
    class VillageWindow : Window
    {
        int Village;
        Texture2D BuildingSlotTexture;
        BuildingWindow BuildingWindow;

        public VillageWindow(string Name, ContentManager Content, int X, int Y, IBuildingInformation BuildingInformation) : base(Name, Content, X, Y)
        {
            WindowRectangle.Width = 400;
            WindowRectangle.Height = 500;
            BuildingSlotTexture = Content.Load<Texture2D>("UI/Windows/BuildingSlot");
            BuildingWindow = new BuildingWindow("BuiildingWindow", Content, WindowRectangle.Right + 10, WindowRectangle.Y, BuildingInformation);
            isOpen = false;
        }

        public void setVillage(int Village)
        {
            this.Village = Village;
            isOpen = true;
        }

        public override bool containsPoint(Point p)
        {
            return base.containsPoint(p) || BuildingWindow.containsPoint(p);
        }

        public void Draw(Dictionary<int, IEntity> Entities, SpriteBatch spriteBatch, float Layer)
        {
            base.Draw(spriteBatch, Layer);

            if (Entities.ContainsKey(Village))
            {
                isOpen = false;
                return;
            }

            IVillage VillageEntitiy = Entities[Village] as IVillage;

            if (VillageEntitiy == null)
                isOpen = false;

            if (!isOpen)
                return;

            this.Name = VillageEntitiy.Name;

            BuildingWindow.setPosition(WindowRectangle.Right + 10, WindowRectangle.Y);
            BuildingWindow.Draw(spriteBatch, Layer);

            Vector2 DrawPosition = new Vector2(WindowRectangle.X+50, WindowRectangle.Y+50);

            for(int x = 0; x < VillageEntitiy.BuildingSlots.X; x++)
            {
                for(int y = 0; y < VillageEntitiy.BuildingSlots.Y; y++)
                {
                    spriteBatch.Draw(texture: BuildingSlotTexture, position: new Vector2(x * 50, y * 50) + DrawPosition, layerDepth: Layer + 0.01f);
                }
            }
        }
    }
}
