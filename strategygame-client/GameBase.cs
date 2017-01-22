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

        Texture2D Cursor;
        Map Map;
        MapRenderer MapRenderer;
        WindowManager Windows;

        NetworkClient Client;

        Dictionary<int, IEntity> Entities;

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
            LastUpdateTicks = DateTime.Now.Ticks;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Cursor = Content.Load<Texture2D>("UI/cursor");

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UpdateNetMessages();

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

            spriteBatch.Draw(Cursor, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), null, Color.White, 0.0f, new Vector2(), 1.0f, SpriteEffects.None, 1.0f);
            MapRenderer.Draw(Map, Entities, spriteBatch, 0.0f);
            Windows.Draw(spriteBatch, 0.2f);

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
