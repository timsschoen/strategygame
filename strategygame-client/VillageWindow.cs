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
        IVillage mVillage;
        int selectedVillage = -1;
        Texture2D mBuildingSlotTexture;
        BuildingWindow mBuildingWindow;

        const int TILESIZE = 50;
        
        /// <summary>
        /// Calculates the position of the tile in row y, and column x, according to tilesize and window position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        Rectangle TileDrawPosition(int x, int y)
        {
            return new Rectangle(mWindowRectangle.X + 50 + x * TILESIZE, mWindowRectangle.Y + 50 + y * TILESIZE, TILESIZE, TILESIZE);
        }

        public VillageWindow(string Name, ContentManager Content, int X, int Y, IBuildingInformation BuildingInformation) : base(Name, Content, X, Y)
        {
            mWindowRectangle.Width = 400;
            mWindowRectangle.Height = 500;
            mBuildingSlotTexture = Content.Load<Texture2D>("UI/Windows/BuildingSlot");
            mBuildingWindow = new BuildingWindow("BuildingWindow", Content, mWindowRectangle.Right + 5, mWindowRectangle.Y, BuildingInformation);
            IsOpen = false;
        }

        public void setVillage(int Village)
        {
            this.selectedVillage = Village;
            IsOpen = true;
        }

        public override bool containsPoint(Point p)
        {
            return (base.containsPoint(p) || mBuildingWindow.containsPoint(p));
        }

        public void update(Dictionary<int, IEntity> Entities)
        {
            base.update();

            if (!Entities.ContainsKey(selectedVillage))
            {
                IsOpen = false;
                return;
            }

            IVillage VillageEntitiy = Entities[selectedVillage] as IVillage;

            if (VillageEntitiy == null)
                IsOpen = false;

            mVillage = VillageEntitiy;

            mBuildingWindow.update();
        }

        public override void draw(SpriteBatch spriteBatch, float Layer)
        {            
            if (!IsOpen || mVillage == null)
                return;

            base.draw(spriteBatch, Layer);

            this.mName = mVillage.Name;

            mBuildingWindow.setPosition(mWindowRectangle.Right + 5, mWindowRectangle.Y);
            mBuildingWindow.draw(mVillage, spriteBatch, Layer);
            
            for(int x = 0; x < mVillage.BuildingSlots.X; x++)
            {
                for(int y = 0; y < mVillage.BuildingSlots.Y; y++)
                {
                    spriteBatch.Draw(texture: mBuildingSlotTexture, destinationRectangle: TileDrawPosition(x, y), layerDepth: Layer + 0.01f);
                }
            }
        }

        public override void handleMouseClick(Point p)
        {
            if (!IsOpen || mVillage == null)
                return;

            for (int x = 0; x < mVillage.BuildingSlots.X; x++)
            {
                for (int y = 0; y < mVillage.BuildingSlots.Y; y++)
                {
                    if(TileDrawPosition(x, y).Contains(p))
                    {
                        mBuildingWindow.setSlot(y*Village.MaxBuildingsInOneRow + x);
                        return;
                    }
                }
            }
        }
    }
}
