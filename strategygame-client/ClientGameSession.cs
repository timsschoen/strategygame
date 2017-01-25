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
    class ClientGameSession
    {
        //Ticks and Time
        long Ticks = 0;
        long LastUpdateTicks;

        //Speed, set 0 for pause
        int GameSpeed = 1;

        Map Map;
        MapRenderer MapRenderer;
        WindowManager Windows;
        MouseHandler MouseHandler;

        Dictionary<int, IEntity> Entities;

        ContentManager Content;
        GameConfiguration Configuration;

        ILogger Logger;

        public void Stop()
        {

        }

        public ClientGameSession(GameConfiguration Configuration, GraphicsDevice GraphicsDevice, ContentManager Content, INetworkSender NetworkSender, ILogger Logger, int ScreenWidth, int ScreenHeight)
        {
            Map = Map.LoadFromFolder(GraphicsDevice, Content.RootDirectory + "/Maps/1");
            Windows = new WindowManager(Content, Configuration.BuildingInformation);
            MapRenderer = new MapRenderer(Content, Map, ScreenWidth, ScreenHeight);
            MouseHandler = new MouseHandler(OnMouseClick, OnSelection, Content);
            LastUpdateTicks = DateTime.Now.Ticks;

            this.Content = Content;
            this.Configuration = Configuration;
            this.Logger = Logger;

            Entities = new Dictionary<int, IEntity>();                 
                   
        }

        public void Update()
        {
            MouseHandler.Update();

            Ticks += GameSpeed * (DateTime.Now.Ticks - LastUpdateTicks);
            
            MapRenderer.Update();
            Windows.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            MouseHandler.Draw(spriteBatch, 1f);
            MapRenderer.Draw(Map, Entities, spriteBatch, 0.0f);
            Windows.Draw(Entities, spriteBatch, 0.2f);
        }

        public void HandleNetworkMessage(IMessage newMessage)
        {
            if (newMessage is EntityMessage)
            {
                EntityMessage EntityMessage = (EntityMessage)newMessage;
                EntityMessage.Entity.ClientInitialize();
                Entities.Add(EntityMessage.EntityID, EntityMessage.Entity);
            }
        }

        public void OnMouseClick(Point ClickPos)
        {
            if (!Windows.ContainsPixel(ClickPos))
            {
                int? EntityClicked = MapRenderer.getClickedEntity(ref Entities, ClickPos);

                if (EntityClicked != null)
                {
                    Logger.Log(LogPriority.Normal, "EntityClicked", "ID: " + EntityClicked + ", Name: " + Entities[EntityClicked.Value].Name + ", Owner: " + Entities[EntityClicked.Value].Owner);

                    if (!Entities.ContainsKey(EntityClicked.Value))
                        return;

                    IEntity Entity = Entities[EntityClicked.Value];

                    if (Entity is IVillage)
                        Windows.VillageWindow.setVillage(EntityClicked.Value);
                }
                else
                {
                    Point ClickedTile = MapRenderer.getClickedHex(ClickPos);
                    Logger.Log(LogPriority.Normal, "TileClicked", "Position: " + ClickedTile.X + "," + ClickedTile.Y);
                }
            }
        }

        public void OnSelection(Point A, Point B)
        {

        }
    }
}
