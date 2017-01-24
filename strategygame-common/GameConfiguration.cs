using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    public class GameConfiguration
    {
        public BuildingInformation BuildingInformation;
    }

    public class GameConfigurationMessage : BaseMessage
    {
        public GameConfiguration Configuration {get;set;}
    }
}
