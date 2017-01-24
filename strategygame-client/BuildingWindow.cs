using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using strategygame_common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_client
{
    class BuildingWindow : Window
    {
        int SelectedSlot;
        Texture2D BuildingSlotTexture;
        IBuildingInformation BuildingInformation;

        public BuildingWindow(string Name, ContentManager Content, int X, int Y, IBuildingInformation BuildingInformation) : base(Name, Content, X, Y)
        {
            WindowRectangle.Width = 200;
            WindowRectangle.Height = 300;
            isOpen = false;
            BuildingSlotTexture = Content.Load<Texture2D>("UI/Windows/BuildingSlot");
            this.BuildingInformation = BuildingInformation;
        }

        public void setSlot(int SelectedSlot)
        {
            this.SelectedSlot = SelectedSlot;
            isOpen = true;
        }

        public void Draw(IVillage Village, SpriteBatch spriteBatch, float Layer)
        {
            base.Draw(spriteBatch, Layer);

            if (Village == null)
                isOpen = false;

            if (Village.Buildings.Length <= SelectedSlot)
                isOpen = false;

            if (!isOpen)
                return;

            Point BuildingAndLevel = Village.Buildings[SelectedSlot];

            if(BuildingAndLevel == null)
            {
                //empty Building Slot
                Name = "Leerer Gebäude-Slot";
                WindowRectangle.Height = 500;
            }
            else
            {
                //show Building Information
                Name = BuildingInformation.getBuildingInfo(BuildingAndLevel.X).Name;
                WindowRectangle.Height = 300;

                List<int> BuildableBuildings = BuildingInformation.BuildableBuildings(Village.Buildings, Village.CellType);

            }
        }

        private void RenderBuildingOption(SpriteBatch spriteBatch, float Layer, int BuildingType, IVillage Village)
        {

        }
    }
}
