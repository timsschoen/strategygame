using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using strategygame_common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace strategygame_client
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameBase : Game
    {
        enum GameState
        {
            Menu,
            Lobby,
            InGame
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        Map Map;
        MapRenderer MapRenderer;
        WindowManager Windows;
        MouseHandler MouseHandler;

        NetworkClient Client;

        Dictionary<int, IEntity> Entities;

        ILogger Logger = new ConsoleLogger();

        //Ticks and Time
        long Ticks = 0;
        long LastUpdateTicks;

        //Speed, set 0 for pause
        int GameSpeed = 1;

        public GameBase()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 1500;
            Content.RootDirectory = "Content";
            Entities = new Dictionary<int, IEntity>();
            Client = new NetworkClient(new ConsoleLogger());
            Thread.Sleep(200); 
            Client.Connect("127.0.0.1", 6679);            
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            Client.Stop();
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            Map = Map.LoadFromFolder(this.GraphicsDevice, Content.RootDirectory + "/Maps/1");
            Windows = new WindowManager(Content);
            MapRenderer = new MapRenderer(Content, Map, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            MouseHandler = new MouseHandler(OnMouseClick, OnSelection, Content);
            LastUpdateTicks = DateTime.Now.Ticks;

            Mouse.WindowHandle = Window.Handle;
        }

        public void OnMouseClick(Point ClickPos)
        {
            if(!Windows.ContainsPixel(ClickPos))
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

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdateNetMessages();
            MouseHandler.Update();

            Ticks += GameSpeed * (DateTime.Now.Ticks - LastUpdateTicks);

            // TODO: Add your update logic here
            MapRenderer.Update(gameTime);
            Windows.Update();

            base.Update(gameTime);
        }
        
        private void UpdateNetMessages()
        {
            IMessage newMessage;
            if ((newMessage = Client.TryGetMessage()) != null)
            {
                if(newMessage is EntityMessage)
                {
                    EntityMessage EntityMessage = (EntityMessage)newMessage;
                    EntityMessage.Entity.ClientInitialize();
                    Entities.Add(EntityMessage.EntityID, EntityMessage.Entity);
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.FrontToBack);

            MouseHandler.Draw(spriteBatch, 1f);
            MapRenderer.Draw(Map, Entities, spriteBatch, 0.0f);
            Windows.Draw(spriteBatch, 0.2f);

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
