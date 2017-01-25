using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using strategygame_common;

namespace strategygame_server
{
    class ServerGameSession
    {
        VillageSystem VillageSystem;

        INetworkSender NetworkSender;
        ILogger Logger;

        EntityCollection Entities;

        GameConfiguration Configuration;

        //Ticks and Time
        long Ticks = 0;
        long LastUpdateTicks;

        //Speed, set 0 for pause
        int GameSpeed = 1;

        public ServerGameSession(INetworkSender NetworkSender, ILogger Logger, GameConfiguration Configuration)
        {
            this.Configuration = Configuration;
            Entities = new EntityCollection();
            VillageSystem = new VillageSystem(NetworkSender, Logger, Configuration.BuildingInformation);
            LastUpdateTicks = DateTime.Now.Ticks;
            this.NetworkSender = NetworkSender;
            this.Logger = Logger;
        }

        public void handleNetworkMessage(IMessage Message)
        {
            VillageSystem.handleNetworkMessage(ref Entities, Message);

            if(Message is NewClientConnectedMessage)
            {
                int ID = ((NewClientConnectedMessage)Message).ClientID;
                NetworkSender.sendOverNetwork(new GameConfigurationMessage(ID,Configuration));
            }
        }

        public void Update()
        {
            Ticks += GameSpeed * (DateTime.Now.Ticks - LastUpdateTicks);
            VillageSystem.Update(ref Entities, Ticks);
        }
    }
}
