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
        INetworkSender mNetworkSender;
        ILogger mLogger;
        IBuildingInformation mBuildingInformation;

        public VillageSystem(INetworkSender networkSender, ILogger logger, IBuildingInformation buildingInformation)
        {
            this.mLogger = logger;
            this.mNetworkSender = networkSender;
            this.mBuildingInformation = buildingInformation;
        }

        public void Update(ref EntityCollection Entities, long Ticks)
        {
            foreach(KeyValuePair<int, IEntity> KVP in Entities)
            {
                if(KVP.Value is IVillage)
                {
                    bool updateNeeded = false;

                    IVillage village = KVP.Value as IVillage;

                    List<int> indicesToRemove = new List<int>();

                    for(int i = 0; i < village.ParallelConstructions.Count; i++)
                    {
                        if(village.ParallelConstructions[i].End <= Ticks)
                        {
                            indicesToRemove.Add(i);

                            if(village.ParallelConstructions[i].isUpgrading)
                            {
                                village.Buildings[village.ParallelConstructions[i].BuildingSlot].Y++;
                            }
                            else
                            {
                                village.Buildings[village.ParallelConstructions[i].BuildingSlot].Y--;
                            }

                            updateNeeded = true;
                        }
                    }

                    for(int i = 0; i < indicesToRemove.Count; i++)
                    {
                        village.ParallelConstructions.RemoveAt(indicesToRemove[i]);
                    }

                    //todo make count of parallel constructions configurable
                    if(village.ParallelConstructions.Count < 1 && village.ConstructionQueue.Count > 0)
                    {
                        QueuedJob job = village.ConstructionQueue.Dequeue();

                        ConstructionProcess process = new ConstructionProcess();

                        process.BuildingSlot = job.BuildingSlot;
                        process.isUpgrading = job.isUpgrading;
                        process.Start = Ticks;
                        process.End = Ticks + job.Length;

                        village.ParallelConstructions.Add(process);

                        updateNeeded = true;
                    }

                    if(updateNeeded)
                    {
                        mNetworkSender.SendOverNetwork(new EntityMessage(village.Owner, KVP.Key, village));
                    }
                }
            }
        }

        public void handleNetworkMessage(ref EntityCollection entities, IMessage message)
        {
            if(message is BuildingMessage)
            {
                BuildingMessage Msg = (BuildingMessage)message;

                if (!entities.ContainsKey(Msg.VillageID))
                    return;

                IVillage village = (IVillage)entities[Msg.VillageID];

                if (!mBuildingInformation.canBuild(Msg.BuildingType, Msg.Level, village.Buildings, village.CellType, village.Resources))
                    return;
                
                //lets go through with this
                QueuedJob queuedJob = new QueuedJob();
                queuedJob.BuildingSlot = Msg.BuildingPosition;
                queuedJob.Length = mBuildingInformation.getBuildingInfo(Msg.BuildingType).ConstructionTimes[Msg.Level] * 1000;
                queuedJob.isUpgrading = true;

                village.ConstructionQueue.Enqueue(queuedJob);

                village.Buildings[Msg.BuildingPosition].X = Msg.BuildingType;

                village.Resources.Subtract(mBuildingInformation.getResources(Msg.BuildingType, Msg.Level));
                
                //Resend the village
                mNetworkSender.SendOverNetwork(new EntityMessage(village.Owner, Msg.VillageID, village));
            }

            if (message is ClientLoadedMessage)
            {
                //for the time being, spawn a village for test purposes
                int NextID = entities.NextID;

                Resources startRes = new Resources();
                startRes.SetResourceCount(0, 200);
                startRes.SetResourceCount(1, 200);
                startRes.SetResourceCount(2, 200);
                startRes.SetResourceCount(3, 200);

                IEntity newEntity = new Village("TestDorf", message.ClientID, new Vector2(10, 10), new Point(3,6), MapCellType.Flatland, startRes); 
                entities.Add(NextID, newEntity);
                mNetworkSender.SendOverNetwork(new EntityMessage(-1, NextID, newEntity));
            }
        }
    }

    interface IVillageSystem
    {
        void Update(ref EntityCollection entities, long ticks);
        void handleNetworkMessage(ref EntityCollection entities,  IMessage message);
    }
}
