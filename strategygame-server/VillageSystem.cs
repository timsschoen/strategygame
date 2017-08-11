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
            foreach(KeyValuePair<int, IEntity> currentEntity in Entities)
            {
                if(currentEntity.Value is IVillage)
                {
                    bool updateNeeded = false;

                    IVillage village = currentEntity.Value as IVillage;

                    List<int> indicesToRemove = new List<int>();

                    for(int i = 0; i < village.ConstructionProcesses.Count; i++)
                    {
                        if (village.ConstructionProcesses[i].End <= Ticks)
                        {
                            //process over
                            indicesToRemove.Add(i);

                            ConstructionProcess constructionProcess = village.ConstructionProcesses[i];

                            if (constructionProcess.isUpgrading)
                            {
                                village.Buildings[constructionProcess.BuildingSlot].Level++;

                            }
                            else
                            {
                                village.Buildings[constructionProcess.BuildingSlot].Level--;
                            }

                            List<int> indicesToRemoveProduction = new List<int>();
                            for(int j = 0; j < village.ProductionProcesses.Count; j++)
                            {
                                if (village.ProductionProcesses[j].OwnerBuildingSlot == constructionProcess.BuildingSlot)
                                    indicesToRemove.Add(j);
                            }

                            foreach (int index in indicesToRemoveProduction)
                                village.ProductionProcesses.RemoveAt(index);

                            BuildingSlot building = village.Buildings[constructionProcess.BuildingSlot];

                            if (building.Active)
                            {
                                List<IBuildingEffect> BuildingEffects = mBuildingInformation.getBuildingInfo(building.Type).BuildingEffects[building.Level - 1];

                                if (BuildingEffects != null)
                                {
                                    foreach (IBuildingEffect effect in BuildingEffects)
                                    {
                                        if (effect is ProductionEffect)
                                        {
                                            ProductionProcess process = ProductionProcess.FromProduction(effect as ProductionEffect, Ticks, constructionProcess.BuildingSlot);
                                            village.ProductionProcesses.Add(process);
                                        }
                                        else if (effect is KeyValueEffect)
                                        {
                                            KeyValueEffect effectAsKVE = effect as KeyValueEffect;
                                            if (!village.Attributes.ContainsKey(effectAsKVE.Key))
                                                village.Attributes.Add(effectAsKVE.Key, effectAsKVE.Value);

                                            village.Attributes[effectAsKVE.Key] = effectAsKVE.Value;

                                        }
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < indicesToRemove.Count; i++)
                    {
                        village.ConstructionProcesses.RemoveAt(indicesToRemove[i]);
                    }

                    indicesToRemove.Clear();

                    for (int i = 0; i < village.ProductionProcesses.Count; i++)
                    {
                        if(village.ProductionProcesses[i].End <= Ticks)
                        {
                            //process over
                            indicesToRemove.Add(i);
                            
                            if(village.ProductionProcesses[i] is ProductionProcess)
                            {
                                ProductionProcess productionProcess = village.ProductionProcesses[i] as ProductionProcess;

                                village.Resources.Add(productionProcess.Out);

                                if (productionProcess.Repeating)
                                {
                                    if (village.Resources.ContainsMoreThan(productionProcess.In))
                                    {
                                        if (village.Buildings[productionProcess.OwnerBuildingSlot].Active)
                                        {
                                            village.Resources.Subtract(productionProcess.In);
                                            productionProcess.End = Ticks + productionProcess.Length;
                                            productionProcess.Start = Ticks;
                                            village.ProductionProcesses.Add(productionProcess);
                                        }
                                    }
                                    else
                                    {
                                        //not enough resources to start the process again
                                        village.Buildings[productionProcess.OwnerBuildingSlot].Active = false;
                                    }                                    
                                }
                            }

                            updateNeeded = true;
                        }
                    }

                    for(int i = 0; i < indicesToRemove.Count; i++)
                    {
                        village.ProductionProcesses.RemoveAt(indicesToRemove[i]);
                    }

                    //todo make count of parallel constructions configurable
                    if(village.ConstructionProcesses.Count < 1 && village.ConstructionQueue.Count > 0)
                    {
                        QueuedJob job = village.ConstructionQueue.Dequeue();

                        //start the process
                        ConstructionProcess process = new ConstructionProcess();

                        process.BuildingSlot = job.BuildingSlot;
                        process.isUpgrading = job.isUpgrading;
                        process.Start = Ticks;
                        process.End = Ticks + job.Length;

                        village.ConstructionProcesses.Add(process);

                        updateNeeded = true;
                    }

                    if(updateNeeded)
                    {
                        mNetworkSender.SendOverNetwork(new EntityMessage(village.Owner, currentEntity.Key, village));
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

                if(village.Buildings[Msg.BuildingPosition] == null)
                {
                    village.Buildings[Msg.BuildingPosition] = new BuildingSlot();
                    village.Buildings[Msg.BuildingPosition].Type = Msg.BuildingType;
                }

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
