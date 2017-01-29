﻿using System;
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

        public VillageWindow(string Name, ContentManager Content, int X, int Y, IBuildingInformation BuildingInformation, GraphicsDevice graphicsDevice) : base(Name, Content, X, Y)
        {
            mWindowRectangle.Width = 400;
            mWindowRectangle.Height = 500;
            mBuildingSlotTexture = Content.Load<Texture2D>("UI/Windows/BuildingSlot");
            mBuildingWindow = new BuildingWindow("BuildingWindow", Content, mWindowRectangle.Right + 5, mWindowRectangle.Y, BuildingInformation, graphicsDevice);
            IsOpen = false;
        }

        public void setVillage(int Village)
        {
            this.selectedVillage = Village;
            IsOpen = true;
        }

        public override bool ContainsScreenPoint(Point p)
        {
            return (base.ContainsScreenPoint(p) || mBuildingWindow.ContainsScreenPoint(p));
        }

        public void Update(Dictionary<int, IEntity> Entities)
        {
            base.Update();

            if (!Entities.ContainsKey(selectedVillage))
            {
                IsOpen = false;
                return;
            }

            IVillage VillageEntitiy = Entities[selectedVillage] as IVillage;

            if (VillageEntitiy == null)
                IsOpen = false;

            mVillage = VillageEntitiy;

            mBuildingWindow.Update();
        }

        public override void Draw(ISpriteRenderer spriteRenderer, float layerDepth)
        {            
            if (!IsOpen || mVillage == null)
                return;

            base.Draw(spriteRenderer, layerDepth);

            this.mName = mVillage.Name;

            mBuildingWindow.SetWindowPosition(mWindowRectangle.Right + 5, mWindowRectangle.Y);
            mBuildingWindow.draw(mVillage, spriteRenderer, layerDepth);
            
            for(int x = 0; x < mVillage.BuildingSlots.X; x++)
            {
                for(int y = 0; y < mVillage.BuildingSlots.Y; y++)
                {
                    spriteRenderer.Draw(mBuildingSlotTexture, TileDrawPosition(x, y), layerDepth: layerDepth + 0.01f);
                }
            }
        }

        public override void HandleMouseClick(Point p)
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
