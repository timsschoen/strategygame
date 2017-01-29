using Microsoft.Xna.Framework;
using strategygame_common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace strategygame_server
{
    class VillageSystem : IVillageSystem
    {
        INetworkSender NetworkSender;
        ILogger Logger;
        IBuildingInformation BuildingInformation;

        public VillageSystem(INetworkSender NetworkSender, ILogger Logger, IBuildingInformation BuildingInformation)
        {
            this.Logger = Logger;
            this.NetworkSender = NetworkSender;
            this.BuildingInformation = BuildingInformation;
        }

        public void Update(ref EntityCollection Entities, long Ticks)
        {

        }

        public void handleNetworkMessage(ref EntityCollection Entities, IMessage Message)
        {
            if(Message is BuildingMessage)
            {
                BuildingMessage Msg = (BuildingMessage)Message;
            }

            if (Message is ClientLoadedMessage)
            {
                //for the time being, spawn a village for test purposes
                int NextID = Entities.NextID;

                Resources startRes = new Resources();
                startRes.SetResourceCount(0, 200);
                startRes.SetResourceCount(1, 200);
                startRes.SetResourceCount(2, 200);
                startRes.SetResourceCount(3, 200);

                IEntity newEntity = new Village("TestDorf", Message.ClientID, new Vector2(10, 10), new Point(3,6), MapCellType.Flatland, startRes); 
                Entities.Add(NextID, newEntity);
                NetworkSender.SendOverNetwork(new EntityMessage(-1, NextID, newEntity));
            }
        }
    }

    interface IVillageSystem
    {
        void Update(ref EntityCollection Entities, long Ticks);
        void handleNetworkMessage(ref EntityCollection Entities,  IMessage Message);
    }
}
