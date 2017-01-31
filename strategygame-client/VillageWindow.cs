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

        INetworkSender mNetworkSender;

        const int TILESIZE = 60;
        
        /// <summary>
        /// Calculates the position of the tile in row y, and column x, according to tilesize and window position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        Rectangle TileDrawPosition(int x, int y)
        {
            return new Rectangle(mWindowRectangle.X + 25 + x * TILESIZE, mWindowRectangle.Y + 75 + y * TILESIZE, TILESIZE, TILESIZE);
        }

        public VillageWindow(string Name, ContentManager Content, int X, int Y, IBuildingInformation BuildingInformation, GraphicsDevice graphicsDevice, INetworkSender networkSender) : base(Name, Content, X, Y)
        {
            mWindowRectangle.Width = 400;
            mWindowRectangle.Height = 500;
            mBuildingSlotTexture = Content.Load<Texture2D>("UI/Windows/BuildingSlot");
            mBuildingWindow = new BuildingWindow("BuildingWindow", Content, mWindowRectangle.Right + 5, mWindowRectangle.Y, BuildingInformation, graphicsDevice);
            mBuildingWindow.OnBuildingUpgrade += BuildingUpgradeHandler;
            mNetworkSender = networkSender;

            IsOpen = false;
        }

        private void BuildingUpgradeHandler(int slot, int buildingType, int buildingLevel)
        {
            BuildingMessage message = new BuildingMessage();
            message.BuildingPosition = slot;
            message.BuildingType = buildingType;
            message.Level = buildingLevel;
            message.VillageID = selectedVillage;

            mNetworkSender.SendOverNetwork(message);
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
            mBuildingWindow.Draw(mVillage, spriteRenderer, layerDepth);

            Vector2 drawingOffset = new Vector2(mWindowRectangle.X, mWindowRectangle.Y);

            List<Tuple<int, string>> resourceList = mVillage.Resources.GetStringRepresentation();
            int resourceCount = resourceList.Count;
            
            for (int i = 0; i < resourceCount; i++)
            {                
                spriteRenderer.DrawString(resourceList[i].Item2, drawingOffset + new Vector2(270, 80 + i * 25), Color.Black, 1f);
            }

            for (int x = 0; x < mVillage.BuildingSlots.X; x++)
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

            if (mBuildingWindow.ContainsScreenPoint(p))
            {
                mBuildingWindow.HandleMouseClick(p);
            }
            else
            {
                for (int x = 0; x < mVillage.BuildingSlots.X; x++)
                {
                    for (int y = 0; y < mVillage.BuildingSlots.Y; y++)
                    {
                        if (TileDrawPosition(x, y).Contains(p))
                        {
                            mBuildingWindow.setSlot(y * mVillage.BuildingSlots.X + x, mVillage);
                            return;
                        }
                    }
                }
            }
            
        }
    }
}
