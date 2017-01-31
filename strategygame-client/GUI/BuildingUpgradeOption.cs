using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using strategygame_common;
using Microsoft.Xna.Framework.Content;

namespace strategygame_client.GUI
{
    class BuildingUpgradeOption : IGUIElement
    {
        public delegate void BuildClicked(int buildingType, int buildingLevel);

        IVillage mVillage;
        Button mBuildButton;
        SingleBuildingInformation mBuildingInfo;
        int mBuildingLevel;
        Rectangle mPosition;
        int mBuildingType;
        public event BuildClicked OnBuild;

        public BuildingUpgradeOption(ContentManager content, Rectangle position, int buildingType, SingleBuildingInformation buildingInfo, int buildingLevel)
        {
            mPosition = position;
            mBuildingInfo = buildingInfo;
            mBuildingLevel = buildingLevel;
            mPosition.Width = 240;
            mBuildingType = buildingType;
            mBuildButton = new Button(content, new Rectangle(position.X + 130, position.X + position.Height - 10, 80, 30), "Bauen");
            mBuildButton.OnClicked += MBuildButton_OnClicked;
        }

        private void MBuildButton_OnClicked(object sender, EventArgs e)
        {
            if (OnBuild != null)
                OnBuild(mBuildingType, mBuildingLevel);
        }

        public int GetHeight()
        {
            return mPosition.Height;
        }

        public void Draw(Vector2 offSetPosition, ISpriteRenderer spriteRenderer, long ticks, float layerDepth)
        {
            if (mBuildingInfo.ConstructionResources.Count <= mBuildingLevel)
                return;

            IResources resourcesToUpgrade = mBuildingInfo.ConstructionResources[mBuildingLevel];

            List<Tuple<int, string>> resourceList = resourcesToUpgrade.GetStringRepresentation();
            int resourceCount = resourceList.Count;
            mPosition.Height = Math.Max(resourceCount * 15 + 70, 100);

            spriteRenderer.DrawRectanglePrimitive(mPosition, 3, Color.Black, false, 1f);

            for (int i = 0; i < resourceCount; i++)
            {
                Color resourceColor = Color.Black;

                if (mVillage.Resources.GetResourceCount(resourceList[i].Item1) < resourcesToUpgrade.GetResourceCount(resourceList[i].Item1))
                    resourceColor = Color.Red;

                spriteRenderer.DrawString(resourceList[i].Item2, new Vector2(mPosition.X + 130, mPosition.Y + 10 + i * 20), resourceColor, 1f);
            }

            spriteRenderer.DrawString(mBuildingInfo.Name, new Vector2(mPosition.X + 10, mPosition.Y + 10), Color.Black, 1f);

            mBuildButton.Draw(offSetPosition, spriteRenderer, ticks, layerDepth);
        }

        public void SetCurrentVillage(IVillage village)
        {
            this.mVillage = village;
        }

        public void HandleMouseClick(Point clickPosition)
        {
            mBuildButton.HandleMouseClick(clickPosition);
        }
    }
}
