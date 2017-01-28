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
        int mGameSpeed = 1;

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

        public ClientGameSession(GameConfiguration Configuration, GraphicsDevice GraphicsDevice, ContentManager Content, INetworkSender NetworkSender, ILogger Logger, int ScreenWidth, int ScreenHeight)
        {
            mMap = Map.LoadFromFolder(GraphicsDevice, Content.RootDirectory + "/Maps/1");
            mUI = new UIManager(Content, Configuration.BuildingInformation);
            mMapRenderer = new MapRenderer(Content, mMap, ScreenWidth, ScreenHeight);
            mMouseHandler = new MouseHandler(onMouseClick, onSelection, Content);
            mLastUpdateTicks = DateTime.Now.Ticks;

            this.mContent = Content;
            this.mConfiguration = Configuration;
            this.mLogger = Logger;

            mEntities = new Dictionary<int, IEntity>();
        }

        /// <summary>
        /// Updates this game session's game logic
        /// </summary>
        public void update()
        {
            mMouseHandler.update(mUI);

            mTicks += mGameSpeed * (DateTime.Now.Ticks - mLastUpdateTicks);
            
            mMapRenderer.Update();
            mUI.Update(mEntities);
        }

        /// <summary>
        /// Draws the game to the given SpriteBatch
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void draw(SpriteBatch spriteBatch)
        {
            mMouseHandler.draw(spriteBatch, 1f);
            mMapRenderer.Draw(mMap, mEntities, spriteBatch, 0.0f);
            mUI.Draw(spriteBatch, 0.2f);
        }

        public void handleNetworkMessage(IMessage newMessage)
        {
            if (newMessage is EntityMessage)
            {
                EntityMessage EntityMessage = (EntityMessage)newMessage;
                EntityMessage.Entity.ClientInitialize();
                mEntities.Add(EntityMessage.EntityID, EntityMessage.Entity);
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
