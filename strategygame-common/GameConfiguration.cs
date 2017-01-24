using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_common
{
    /// <summary>
    /// Holds all variables which can be configured in lobby or for balancing purposes
    /// </summary>
    public class GameConfiguration
    {
        public BuildingInformation BuildingInformation;
    }

    /// <summary>
    /// Message to hold the current GameConfiguration. To be sent to clients when initilizing the game.
    /// </summary>
    public class GameConfigurationMessage : BaseMessage
    {
        public GameConfiguration Configuration {get;set;}

        public GameConfigurationMessage(GameConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }
    }
}
