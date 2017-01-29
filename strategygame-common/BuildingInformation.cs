using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class BuildingInformation : IBuildingInformation
    {
        public Dictionary<int, SingleBuildingInformation> BuildingData;

        public BuildingInformation()
        {
            BuildingData = new Dictionary<int, SingleBuildingInformation>();
        }

        private bool Fulfills(List<Point> Requirements, Point[] Current)
        {
            for(int i = 0; i < Requirements.Count; i++)
            {
                int BuildingType = Requirements[i].X;

                bool Fulfilled = false;

                for(int j = 0; j < Current.Length; j++)
                {
                    if (Current[j] == null)
                        continue;

                    if (Current[j].X == BuildingType && Current[j].Y >= Requirements[i].Y)
                    {
                        Fulfilled = true;
                        break;
                    }
                }

                if (!Fulfilled)
                    return false;
            }

            return true;
        }
        
        public bool canBuild(int BuildingType, int Level, Point[] BuildingsInVillage, MapCellType MapCellType, IResources Resources)
        {
            //Building Exists
            if (!BuildingData.ContainsKey(BuildingType))
                return false;

            SingleBuildingInformation info = BuildingData[BuildingType];

            //MapCell requirement
            if (info.CellType != null && info.CellType != MapCellType)
                return false;

            //test MaxLevel
            if (info.ConstructionResources.Count <= Level)
                return false;

            //Dependencies
            if (!Fulfills(info.Dependencies, BuildingsInVillage))
                return false;

            IResources NeededResources = getResources(BuildingType, Level);

            if (NeededResources == null || !Resources.ContainsMoreThan(NeededResources))
                return false;

            return true;
            
        }

        public List<int> BuildableBuildings(Point[] BuildingsInVillage, MapCellType MapCellType)
        {
            List<int> Result = new List<int>();

            foreach(KeyValuePair<int, SingleBuildingInformation> Building in BuildingData)
            {
                SingleBuildingInformation info = Building.Value;

                //MapCell requirement
                if (info.CellType != null && info.CellType != MapCellType)
                    continue;

                //Dependencies
                if (!Fulfills(info.Dependencies, BuildingsInVillage))
                    continue;

                Result.Add(Building.Key);
            }

            return Result;
        }

        public IResources getResources(int Building, int Level)
        {
            if (!BuildingData.ContainsKey(Building))
                return null;

            if (BuildingData[Building].ConstructionResources.Count <= Level)
                return null;

            return BuildingData[Building].ConstructionResources[Level];
        }

        public List<Point> getDependencies(int Building)
        {
            if (!BuildingData.ContainsKey(Building))
                return null;

            return BuildingData[Building].Dependencies;
        }

        public SingleBuildingInformation getBuildingInfo(int BuildingType)
        {
            if (!BuildingData.ContainsKey(BuildingType))
                return null;

            return BuildingData[BuildingType];
        }
    }

    public class SingleBuildingInformation
    {
        public string Name;
        public string Description;
        public List<Point> Dependencies;
        public MapCellType? CellType;

        [JsonConverter(typeof(ConcreteListJsonConverter<IResources, Resources>))]
        public List<IResources> ConstructionResources;

        [JsonConverter(typeof(ConcreteListJsonConverter<IResources, Resources>))]
        public List<IResources> BuildingEffects;
    }

    public interface IBuildingInformation
    {
        bool canBuild(int BuildingType, int Level, Point[] BuildingsInVillage, MapCellType MapCellType, IResources Resources);
        List<int> BuildableBuildings(Point[] BuildingsInVillage, MapCellType MapCellType);
        IResources getResources(int BuildingType, int Level);
        List<Point> getDependencies(int BuildingType);
        SingleBuildingInformation getBuildingInfo(int BuildingType);
    }    
}
