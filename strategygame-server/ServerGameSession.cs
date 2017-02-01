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
        long LastClientSyncTick;

        //Speed, set 0 for pause
        float GameSpeed = 1;

        public ServerGameSession(INetworkSender NetworkSender, ILogger Logger, GameConfiguration Configuration)
        {
            this.Configuration = Configuration;
            Entities = new EntityCollection();
            VillageSystem = new VillageSystem(NetworkSender, Logger, Configuration.BuildingInformation);
            DateTime now = DateTime.Now;
            LastUpdateTicks = (long)((now - new DateTime(1970, 1, 1)).TotalMilliseconds);
            this.NetworkSender = NetworkSender;
            this.Logger = Logger;
        }

        public void handleNetworkMessage(IMessage Message)
        {
            VillageSystem.handleNetworkMessage(ref Entities, Message);

            if(Message is NewClientConnectedMessage)
            {
                int ID = ((NewClientConnectedMessage)Message).ClientID;
                NetworkSender.SendOverNetwork(new GameConfigurationMessage(ID,Configuration));
            }
        }

        public void Update()
        {
            DateTime now = DateTime.Now;
            Ticks += (long)((now - new DateTime(1970, 1, 1)).TotalMilliseconds) - LastUpdateTicks;
            LastUpdateTicks = (long)((now - new DateTime(1970, 1, 1)).TotalMilliseconds);

            VillageSystem.Update(ref Entities, Ticks);

            if(Ticks - LastClientSyncTick > 1000)
            {
                LastClientSyncTick = Ticks;

                NetworkSender.SendOverNetwork(new TickMessage(Ticks,GameSpeed));
            }
        }
    }
}
