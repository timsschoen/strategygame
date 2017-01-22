﻿using Microsoft.Xna.Framework;
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

            if (Message is NewClientConnectedMessage)
            {
                int NextID = Entities.NextID;
                IEntity newEntity = new Village("TestDorf", Message.ClientID, new Vector2(0, 0)); 
                Entities.Add(NextID, newEntity);
                NetworkSender.sendOverNetwork(new EntityMessage(-1, NextID, newEntity));
            }
        }
    }

    interface IVillageSystem
    {
        void Update(ref EntityCollection Entities, long Ticks);
        void handleNetworkMessage(ref EntityCollection Entities,  IMessage Message);
    }
}
