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

        public BuildingWindow(string name, ContentManager content, int x, int y, IBuildingInformation buildingInformation) : base(name, content, x, y)
        {
            mWindowRectangle.Width = 300;
            mWindowRectangle.Height = 300;
            IsOpen = false;
            mBuildingSlotTexture = content.Load<Texture2D>("UI/Windows/BuildingSlot");
            this.mBuildingInformation = buildingInformation;
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
        /// <param name="spriteBatch"></param>
        /// <param name="Layer"></param>
        public void draw(IVillage village, SpriteBatch spriteBatch, float Layer)
        {
            base.Draw(spriteBatch, Layer);

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

                Texture2D BuildingOptions = getAllBuildingOptions(village, spriteBatch.GraphicsDevice);

                spriteBatch.Draw(BuildingOptions, new Rectangle(mWindowRectangle.X, mWindowRectangle.Y + 50, 300, 450), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Layer+0.05f);

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
        Texture2D getAllBuildingOptions(IVillage village, GraphicsDevice device)
        {
            //TODO: dont create a spritebatch every frame

            RenderTarget2D renderTarget = new RenderTarget2D(device, 280, 500, false,
                device.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            device.SetRenderTarget(renderTarget);

            device.Clear(Color.Transparent);
            SpriteBatch spriteBatch = new SpriteBatch(device);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            List<int> BuildableBuildings = mBuildingInformation.BuildableBuildings(village.Buildings, village.CellType);

            for(int i = 0; i < BuildableBuildings.Count; i++)
            {
                SingleBuildingInformation info = mBuildingInformation.getBuildingInfo(BuildableBuildings[i]);

                renderBuildingInformation(spriteBatch, new Vector2(50, 50 + 100 * i), village, info, 1);
            }

            spriteBatch.End();
            device.SetRenderTarget(null);
            return renderTarget;
        }

        /// <summary>
        /// Renders information about the given buildingtype at the given level to the given spritebatch, starting (top-left-corner) at the renderStart-Point
        /// </summary>
        /// <param name="spriteBatch">a initilized SpriteBatch-object to draw to</param>
        /// <param name="renderStart">vector to be the top-left corner of the information about the building</param>
        /// <param name="village">The village we are in</param>
        /// <param name="buildingInfo">Information about the building to be built</param>
        /// <param name="buildingLevel">desired level of the building</param>
        private void renderBuildingInformation(SpriteBatch spriteBatch, Vector2 renderStart, IVillage village, SingleBuildingInformation buildingInfo, int buildingLevel)
        {
            spriteBatch.DrawString(mFont, buildingInfo.Name, renderStart + new Vector2(5,5), Color.Black);
        }
    }
}
