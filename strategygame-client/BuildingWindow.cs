using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using strategygame_client.GUI;
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
        public delegate void BuildingUpgradeClicked(int slot, int buildingType, int buildingLevel);

        int mSelectedSlot;
        Texture2D mBuildingSlotTexture;
        IBuildingInformation mBuildingInformation;

        //For Rendering scrolling box
        RenderTarget2D mRenderTarget;
        GraphicsDevice mGraphicsDevice;
        ISpriteRenderer mScrollBoxSpriteRenderer;

        public event BuildingUpgradeClicked OnBuildingUpgrade;

        BuildingUpgradeList mUpgradeList;

        public BuildingWindow(string name, ContentManager content, int x, int y, IBuildingInformation buildingInformation, GraphicsDevice graphicsDevice) : base(name, content, x, y)
        {
            mWindowRectangle.Width = 300;
            mWindowRectangle.Height = 500;
            IsOpen = false;
            mBuildingSlotTexture = content.Load<Texture2D>("UI/Windows/BuildingSlot");
            this.mBuildingInformation = buildingInformation;
            mRenderTarget = new RenderTarget2D(graphicsDevice, mWindowRectangle.Width, mWindowRectangle.Height-50, false,
                 graphicsDevice.PresentationParameters.BackBufferFormat,
                 DepthFormat.Depth24);
            mGraphicsDevice = graphicsDevice;
            mScrollBoxSpriteRenderer = new SpriteBatchRenderer(new SpriteBatch(mGraphicsDevice), content.Load<SpriteFont>("Default"));

            mUpgradeList = new BuildingUpgradeList(content);
            mUpgradeList.OnBuildClicked += BuildingUpgradeClickedEventHandler;
            
        }

        private void BuildingUpgradeClickedEventHandler(int buildingType, int buildingLevel)
        {
            if(OnBuildingUpgrade != null)
            {
                OnBuildingUpgrade(mSelectedSlot, buildingType, buildingLevel);
            }
            IsOpen = false;
        }

        /// <summary>
        /// Sets this window's selected slot the given slot and opens it, if closed
        /// </summary>
        /// <param name="selectedSlot">the building slot to display information about</param>
        public void setSlot(int selectedSlot, IVillage village)
        {
            this.mSelectedSlot = selectedSlot;

            BuildingSlot buildingAndLevel = village.Buildings[mSelectedSlot];

            if (buildingAndLevel == null)
            {
                //empty Building Slot
                mName = "Leerer Gebäude-Slot";
                mUpgradeList.ShowAllNewBuildings(mBuildingInformation, village);
            }
            else
            {
                //show Building Information
                mName = mBuildingInformation.getBuildingInfo(buildingAndLevel.Type).Name;
                mUpgradeList.ShowBuildingUpdate(mBuildingInformation, village, buildingAndLevel);
            }

            IsOpen = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="village"></param>
        /// <param name="spriteRenderer"></param>
        /// <param name="Layer"></param>
        public void Draw(IVillage village, ISpriteRenderer spriteRenderer, float Layer)
        {
            base.Draw(spriteRenderer, Layer);

            if (village == null)
                IsOpen = false;

            if (village.Buildings.Length <= mSelectedSlot)
                IsOpen = false;

            if (!IsOpen)
                return;
            
            Texture2D BuildingOptions = getAllBuildingOptions(village);

            spriteRenderer.Draw(BuildingOptions, new Rectangle(mWindowRectangle.X, mWindowRectangle.Y + 50, mWindowRectangle.Width, mWindowRectangle.Height - 50), Layer + 0.05f);
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

            mUpgradeList.SetVillage(village);
            mUpgradeList.Draw(Vector2.Zero, mScrollBoxSpriteRenderer, 0, 1f);

            mScrollBoxSpriteRenderer.End();
            mGraphicsDevice.SetRenderTarget(null);
            return mRenderTarget;
        }

        public override void HandleMouseClick(Point p)
        {
            if(new Rectangle(mWindowRectangle.X, mWindowRectangle.Y +50, mWindowRectangle.Width, mWindowRectangle.Height -50).Contains(p))
            {
                mUpgradeList.HandleMouseClick(Vector2.Zero, new Point(p.X - mWindowRectangle.X, p.Y - mWindowRectangle.Y - 50));
            }
        }
    }
}
