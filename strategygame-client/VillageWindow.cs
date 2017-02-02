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
        IBuildingInformation mBuildingInformation;
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
            return new Rectangle(mWindowRectangle.X + 25 + x * TILESIZE, mWindowRectangle.Y + 50 + y * TILESIZE, TILESIZE, TILESIZE);
        }

        public VillageWindow(string name, ContentManager content, int X, int Y, IBuildingInformation buildingInformation, GraphicsDevice graphicsDevice, INetworkSender networkSender) : base(name, content, X, Y)
        {
            mWindowRectangle.Width = 400;
            mWindowRectangle.Height = 600;
            mBuildingSlotTexture = content.Load<Texture2D>("UI/Windows/BuildingSlot");
            mBuildingWindow = new BuildingWindow("BuildingWindow", content, mWindowRectangle.Right + 5, mWindowRectangle.Y, buildingInformation, graphicsDevice);
            mBuildingWindow.OnBuildingUpgrade += BuildingUpgradeHandler;
            mNetworkSender = networkSender;
            mBuildingInformation = buildingInformation;

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

        public void Draw(ISpriteRenderer spriteRenderer, float layerDepth, long ticks)
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
                spriteRenderer.DrawString(resourceList[i].Item2, drawingOffset + new Vector2(270, 50 + i * 25), Color.Black, layerDepth + 0.01f);
            }

            int jobcount = 0;
            for(int i = 0; i < mVillage.ParallelConstructions.Count; i++)
            {
                jobcount++;

                string BuildingName = mBuildingInformation.getBuildingInfo(mVillage.Buildings[mVillage.ParallelConstructions[i].BuildingSlot].X).Name;
                int newLevel = mVillage.Buildings[mVillage.ParallelConstructions[i].BuildingSlot].Y;

                spriteRenderer.DrawString("Construction of " + BuildingName + " Level " + (newLevel+1), drawingOffset + new Vector2(25, 420 + jobcount * 25), Color.Black, layerDepth + 0.01f);

                //draw progress bar
                float progress = mVillage.ParallelConstructions[i].interpolate(ticks);
                
                spriteRenderer.DrawRectanglePrimitive(new Rectangle((int)drawingOffset.X + 280, (int)drawingOffset.Y + 420 + jobcount * 25, 102, 20), 1, Color.Black, false, layerDepth + 0.01f);
                spriteRenderer.DrawRectanglePrimitive(new Rectangle((int)drawingOffset.X + 281, (int)drawingOffset.Y + 421 + jobcount * 25, (int)(100* progress), 18), 1, Color.Green, true, layerDepth + 0.01f);
            }
            for (int i = 0; i < mVillage.ConstructionQueue.Count; i++)
            {
                jobcount++;

                string BuildingName = mBuildingInformation.getBuildingInfo(mVillage.Buildings[mVillage.ConstructionQueue.ElementAt(i).BuildingSlot].X).Name;
                int newLevel = mVillage.Buildings[mVillage.ConstructionQueue.ElementAt(i).BuildingSlot].Y;

                spriteRenderer.DrawString("Construction of " + BuildingName + " Level " + newLevel, drawingOffset + new Vector2(25, 420 + jobcount * 25), Color.Black, layerDepth + 0.01f);
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
