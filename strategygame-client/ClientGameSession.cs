﻿using Microsoft.Xna.Framework;
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
        long mTicks = 0;
        long mLastUpdateTicks;

        //Speed, set 0 for pause
        float mGameSpeed = 1;

        Map mMap;
        MapRenderer mMapRenderer;
        UIManager mUI;
        MouseHandler mMouseHandler;

        Dictionary<int, IEntity> mEntities;

        ContentManager mContent;
        GameConfiguration mConfiguration;

        ILogger mLogger;

        public void stop()
        {

        }

        public ClientGameSession(GameConfiguration configuration, GraphicsDevice graphicsDevice, ContentManager content, INetworkSender networkSender, ILogger logger, int screenWidth, int screenHeight)
        {
            mMap = Map.LoadFromFolder(graphicsDevice, content.RootDirectory + "/Maps/1");
            mUI = new UIManager(content, configuration.BuildingInformation, graphicsDevice, networkSender);
            mMapRenderer = new MapRenderer(content, mMap, screenWidth, screenHeight);
            mMouseHandler = new MouseHandler(onMouseClick, onSelection, content);
            DateTime now = DateTime.Now;
            mLastUpdateTicks = (long)(now - new DateTime(1970, 1, 1)).TotalMilliseconds;

            this.mContent = content;
            this.mConfiguration = configuration;
            this.mLogger = logger;

            mEntities = new Dictionary<int, IEntity>();
        }

        /// <summary>
        /// Updates this game session's game logic
        /// </summary>
        public void update()
        {
            mMouseHandler.update(mUI);

            DateTime now = DateTime.Now;
            mTicks += (long)((now - new DateTime(1970, 1, 1)).TotalMilliseconds) - mLastUpdateTicks;
            //mTicks += (long)(mGameSpeed * ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds - mLastUpdateTicks));
            mLastUpdateTicks = (long)((now - new DateTime(1970, 1, 1)).TotalMilliseconds);

            mMapRenderer.Update();
            mUI.Update(mEntities);
        }
        
        /// <summary>
        /// Draws the game to the given SpriteRenderer
        /// </summary>
        /// <param name="spriteRenderer"></param>
        public void draw(ISpriteRenderer spriteRenderer)
        {
            mMouseHandler.draw(spriteRenderer, 1f);
            mMapRenderer.Draw(mMap, mEntities, spriteRenderer, 0.0f);
            mUI.Draw(spriteRenderer, 0.2f, mTicks);
            
        }

        public void handleNetworkMessage(IMessage newMessage)
        {
            if (newMessage is EntityMessage)
            {
                EntityMessage EntityMessage = (EntityMessage)newMessage;
                EntityMessage.Entity.ClientInitialize();

                if (mEntities.ContainsKey(EntityMessage.EntityID))
                    mEntities[EntityMessage.EntityID] = EntityMessage.Entity;
                else
                    mEntities.Add(EntityMessage.EntityID, EntityMessage.Entity);
            }
            else if (newMessage is TickMessage)
            {
                TickMessage TickMessage = (TickMessage)newMessage;

                mTicks = TickMessage.Ticks;
                //mLastUpdateTicks = (long)((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
                mGameSpeed = TickMessage.GameSpeed;
            }
        }

        public void onMouseClick(Point ClickPos)
        {
            int? clickedEntitiyID = mMapRenderer.getClickedEntity(ref mEntities, ClickPos);

            if (clickedEntitiyID != null)
            {
                mLogger.Log(LogPriority.Normal, "EntityClicked", "ID: " + clickedEntitiyID + ", Name: " + mEntities[clickedEntitiyID.Value].Name + ", Owner: " + mEntities[clickedEntitiyID.Value].Owner);

                if (!mEntities.ContainsKey(clickedEntitiyID.Value))
                    return;

                IEntity clickedEntity = mEntities[clickedEntitiyID.Value];

                if (clickedEntity is IVillage)
                    mUI.SelectVillage(clickedEntitiyID.Value);
            }
            else
            {
                Point ClickedTile = mMapRenderer.getClickedHex(ClickPos);
                mLogger.Log(LogPriority.Normal, "TileClicked", "Position: " + ClickedTile.X + "," + ClickedTile.Y);
            }
            
        }

        public void onSelection(Point A, Point B)
        {

        }
    }
}
