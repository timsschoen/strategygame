using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using strategygame_common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_client
{
    class BuildingWindow : Window
    {
        int mSelectedSlot;
        Texture2D mBuildingSlotTexture;
        IBuildingInformation mBuildingInformation;

        //For Rendering scrolling box
        RenderTarget2D mRenderTarget;
        GraphicsDevice mGraphicsDevice;
        ISpriteRenderer mScrollBoxSpriteRenderer;

        public BuildingWindow(string name, ContentManager content, int x, int y, IBuildingInformation buildingInformation, GraphicsDevice graphicsDevice) : base(name, content, x, y)
        {
            mWindowRectangle.Width = 300;
            mWindowRectangle.Height = 300;
            IsOpen = false;
            mBuildingSlotTexture = content.Load<Texture2D>("UI/Windows/BuildingSlot");
            this.mBuildingInformation = buildingInformation;
            mRenderTarget = new RenderTarget2D(graphicsDevice, 280, 500, false,
                 graphicsDevice.PresentationParameters.BackBufferFormat,
                 DepthFormat.Depth24);
            mGraphicsDevice = graphicsDevice;
            mScrollBoxSpriteRenderer = new SpriteBatchRenderer(new SpriteBatch(mGraphicsDevice), content.Load<SpriteFont>("Default"));
        }

        /// <summary>
        /// Sets this window's selected slot the given slot and opens it, if closed
        /// </summary>
        /// <param name="selectedSlot">the building slot to display information about</param>
        public void setSlot(int selectedSlot)
        {
            this.mSelectedSlot = selectedSlot;
            IsOpen = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="village"></param>
        /// <param name="spriteRenderer"></param>
        /// <param name="Layer"></param>
        public void draw(IVillage village, ISpriteRenderer spriteRenderer, float Layer)
        {
            base.Draw(spriteRenderer, Layer);

            if (village == null)
                IsOpen = false;

            if (village.Buildings.Length <= mSelectedSlot)
                IsOpen = false;

            if (!IsOpen)
                return;

            Point buildingAndLevel = village.Buildings[mSelectedSlot];

            if(buildingAndLevel.X == 0)
            {
                //empty Building Slot
                mName = "Leerer Gebäude-Slot";
                mWindowRectangle.Height = 500;

                Texture2D BuildingOptions = getAllBuildingOptions(village);

                spriteRenderer.Draw(BuildingOptions, new Rectangle(mWindowRectangle.X, mWindowRectangle.Y + 50, 300, 450), Layer+0.05f);

            }
            else
            {
                //show Building Information
                mName = mBuildingInformation.getBuildingInfo(buildingAndLevel.X).Name;
                mWindowRectangle.Height = 300;
            }
        }

        /// <summary>
        /// Renders a overview of all buildable buildings on a empty tile in this village
        /// </summary>
        /// <param name="village">the village we are in, needed for information about resources or building dependencies</param>
        /// <param name="device">the game's graphic device</param>
        /// <returns></returns>
        Texture2D getAllBuildingOptions(IVillage village)
        {
            mGraphicsDevice.SetRenderTarget(mRenderTarget);
            mGraphicsDevice.Clear(Color.Transparent);
            mScrollBoxSpriteRenderer.Begin();

            List<int> BuildableBuildings = mBuildingInformation.BuildableBuildings(village.Buildings, village.CellType);

            for(int i = 0; i < BuildableBuildings.Count; i++)
            {
                SingleBuildingInformation info = mBuildingInformation.getBuildingInfo(BuildableBuildings[i]);

                renderBuildingInformation(mScrollBoxSpriteRenderer, new Point(10, 50 + 100 * i), village, info, 1);
            }

            mScrollBoxSpriteRenderer.End();
            mGraphicsDevice.SetRenderTarget(null);
            return mRenderTarget;
        }

        /// <summary>
        /// Renders information about the given buildingtype at the given level to the given spritebatch, starting (top-left-corner) at the renderStart-Point
        /// </summary>
        /// <param name="spriteRenderer">a initilized ISpriteRenderer-object to draw to</param>
        /// <param name="renderStart">vector to be the top-left corner of the information about the building</param>
        /// <param name="village">The village we are in</param>
        /// <param name="buildingInfo">Information about the building to be built</param>
        /// <param name="buildingLevel">desired level of the building</param>     
        /// <returns>Height of the item</returns>    
        private int renderBuildingInformation(ISpriteRenderer spriteRenderer, Point renderStart, IVillage village, SingleBuildingInformation buildingInfo, int buildingLevel)
        {
            if (buildingInfo.ConstructionResources.Count <= buildingLevel)
                return 0;

            IResources resourcesToUpgrade = buildingInfo.ConstructionResources[buildingLevel];

            List<Tuple<int,string>> resourceList = resourcesToUpgrade.GetStringRepresentation();
            int resourceCount = resourceList.Count;
            int itemHeight = Math.Max(resourceCount * 15, 100);
            
            spriteRenderer.DrawRectanglePrimitive(new Rectangle(renderStart, new Point(280, itemHeight)),3, Color.Black, false, 1f);

            for(int i = 0; i < resourceCount; i++)
            {
                Color resourceColor = Color.Black;

                if (village.Resources.GetResourceCount(resourceList[i].Item1) < resourcesToUpgrade.GetResourceCount(resourceList[i].Item1))
                    resourceColor = Color.Red;

                spriteRenderer.DrawString(resourceList[i].Item2, new Vector2(renderStart.X + 100, renderStart.Y + i * 20), resourceColor, 1f);
            }

            spriteRenderer.DrawString(buildingInfo.Name, new Vector2(renderStart.X + 5, renderStart.Y + 5), Color.Black, 1f);

            spriteRenderer.DrawText(buildingInfo.Description, new Rectangle(renderStart.X + 5, renderStart.Y + 20, 100, 50), Color.Black, 1f);

            return itemHeight;
        }
    }
}
