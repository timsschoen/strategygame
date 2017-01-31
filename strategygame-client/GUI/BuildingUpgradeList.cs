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
    class BuildingUpgradeList : IGUIElement
    {
        List<BuildingUpgradeOption> mUpgradeOptions;
        ContentManager mContent;

        public event BuildingUpgradeOption.BuildClicked OnBuildClicked;

        public BuildingUpgradeList(ContentManager content)
        {
            mContent = content;
            mUpgradeOptions = new List<BuildingUpgradeOption>();
        }

        public void Draw(Vector2 offSetPosition, ISpriteRenderer spriteRenderer, long ticks, float layerDepth)
        {
            for (int i = 0; i < mUpgradeOptions.Count; i++)
            {
                mUpgradeOptions[i].Draw(offSetPosition, spriteRenderer, ticks, layerDepth);
            }
        }

        public void HandleMouseClick(Point clickPosition)
        {
            for (int i = 0; i < mUpgradeOptions.Count; i++)
            {
                mUpgradeOptions[i].HandleMouseClick(clickPosition);
            }
        }

        public void SetVillage(IVillage village)
        {
            for(int i = 0; i < mUpgradeOptions.Count; i++)
            {
                mUpgradeOptions[i].SetCurrentVillage(village);
            }
        }

        public void ShowAllNewBuildings(IBuildingInformation buildingInformation, IVillage village)
        {
            mUpgradeOptions.Clear();

            List<int> BuildableBuildings = buildingInformation.BuildableBuildings(village.Buildings, village.CellType);

            int currentHeight = 0;

            for (int i = 0; i < BuildableBuildings.Count; i++)
            {
                SingleBuildingInformation info = buildingInformation.getBuildingInfo(BuildableBuildings[i]);
                BuildingUpgradeOption upgradeOption = new BuildingUpgradeOption(mContent, new Rectangle(10, 50 + currentHeight, 240, 100), BuildableBuildings[i], buildingInformation.getBuildingInfo(BuildableBuildings[i]), 1);
                upgradeOption.OnBuild += UpgradeOption_OnBuild;
                currentHeight += upgradeOption.GetHeight();
                mUpgradeOptions.Add(upgradeOption);
            }
        }

        private void UpgradeOption_OnBuild(int buildingType, int buildingLevel)
        {
            if (OnBuildClicked != null)
                OnBuildClicked(buildingType, buildingLevel);
        }

        public void ShowBuildingUpdate(IBuildingInformation buildingInformation, IVillage village, Point currentBuilding)
        {
            mUpgradeOptions.Clear();
        }        
    }
}
