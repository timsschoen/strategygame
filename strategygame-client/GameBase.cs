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

        GameState State = GameState.Lobby;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        NetworkClient Client;

        ClientGameSession GameSession;

        ILogger Logger = new ConsoleLogger();
        
        public GameBase()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 1500;
            Content.RootDirectory = "Content";
            Client = new NetworkClient(new ConsoleLogger());
            Thread.Sleep(200); 
            Client.Connect("127.0.0.1", 6679);            
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            Client.Stop();

            if(GameSession != null)
                GameSession.stop();
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
                        
            Mouse.WindowHandle = Window.Handle;
        }

        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);    
                    
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
            base.Update(gameTime);
            UpdateNetMessages();

            if (!IsActive)
                return;

            if(State == GameState.InGame)
                GameSession.update();

        }
        
        private void UpdateNetMessages()
        {
            IMessage newMessage;
            if ((newMessage = Client.TryGetMessage()) != null)
            {
                if(State == GameState.InGame)
                    GameSession.handleNetworkMessage(newMessage);
                else if(newMessage is GameConfigurationMessage)
                {
                    GameSession = new ClientGameSession(((GameConfigurationMessage)newMessage).Configuration, 
                        GraphicsDevice,
                        Content, 
                        Client, 
                        Logger, 
                        graphics.PreferredBackBufferWidth, 
                        graphics.PreferredBackBufferHeight);
                    State = GameState.InGame;
                    Client.sendOverNetwork(new ClientLoadedMessage());
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

            if(State == GameState.InGame)
                GameSession.draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
